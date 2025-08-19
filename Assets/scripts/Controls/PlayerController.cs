using UnityEngine;
using UnityEngine.InputSystem;
using Dreamteck.Forever;

public class PlayerController : MonoBehaviour
{
    [Header("Movement (X)")]
    [SerializeField] private Runner runner;
    [SerializeField] private float strafeSpeed = 5f;
    [SerializeField] private float maxOffset = 2f;
    [SerializeField] private float runDeadzone = 0.05f;

    [Header("Jump (Y)")]
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float gravity = -40f;
    [SerializeField] private float groundY = 0f;
    [SerializeField] private float groundSnap = 0.01f;

    [Header("Jump UX")]
    [SerializeField] private float jumpBufferTime = 0.10f;     // keep: buffer press slightly before landing
    [SerializeField] private float groundedStableTime = 0.05f; // must be on ground this long before next jump

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool alwaysRun = true;

    private const string RunningBool = "isRunning";
    private const string JumpTrigger = "isJumping";
    private static readonly int RunningBoolHash = Animator.StringToHash(RunningBool);
    private static readonly int JumpTriggerHash = Animator.StringToHash(JumpTrigger);

    // Gate: require Jump animation to finish before next jump
    [Header("Jump Animation Gate")]
    [SerializeField] private string jumpStateName = "Jump"; // exact name of your Jump state in Animator
    private bool _jumpInProgress;

    private InputController _input;
    private float _inputX;
    private float _yVel;

    private bool _jumpRequested;
    private float _lastJumpPressedTime;
    private float _groundedEnteredTime;
    private bool _wasGrounded;

    private void Awake()
    {
        if (runner == null) runner = GetComponent<Runner>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;

        _input = new InputController();
        _input.MovementReceived += OnMovementReceived;
        _input.JumpPerformed += OnJumpPerformed;
        _input.JumpCanceled += OnJumpCanceled;

        // Respect Runner's initial vertical offset as ground
        groundY = runner.motion.offset.y;
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.MovementReceived -= OnMovementReceived;
            _input.JumpPerformed -= OnJumpPerformed;
            _input.JumpCanceled -= OnJumpCanceled;
            _input.Dispose();
        }
    }

    private void OnMovementReceived(Vector2 movement) => _inputX = movement.x;

    private void Update()
    {
        if (runner == null) return;
        var motion = runner.motion;

        // ----- Horizontal (X) -----
        float targetX = Mathf.Clamp(_inputX * maxOffset, -maxOffset, maxOffset);
        motion.offset = new Vector2(
            Mathf.MoveTowards(motion.offset.x, targetX, strafeSpeed * Time.deltaTime),
            motion.offset.y
        );

        // ----- Ground check BEFORE vertical integration -----
        bool grounded = (motion.offset.y <= groundY + groundSnap) && _yVel <= 0f;
        float now = Time.time;

        if (grounded)
        {
            if (!_wasGrounded) _groundedEnteredTime = now;
        }

        // STRICT: we only allow a new jump when BOTH:
        // 1) jump animation has finished, AND
        // 2) we are stably grounded
        bool groundedStable = grounded && (now - _groundedEnteredTime) >= groundedStableTime;
        bool canJumpNow = groundedStable && !_jumpInProgress;

        // Consume buffered press if valid
        if (_jumpRequested && (now - _lastJumpPressedTime) <= jumpBufferTime && canJumpNow)
        {
            DoJump();
            _jumpRequested = false;
            grounded = false; // we just left the ground
        }
        else if ((now - _lastJumpPressedTime) > jumpBufferTime)
        {
            _jumpRequested = false;
        }

        // ----- Vertical (Y) integration -----
        if (!grounded)
        {
            _yVel += gravity * Time.deltaTime;
            float newY = motion.offset.y + _yVel * Time.deltaTime;

            if (newY < groundY)
            {
                newY = groundY;
                _yVel = 0f;
                grounded = true;
                _groundedEnteredTime = now; // start stable-ground timer
            }

            motion.offset = new Vector2(motion.offset.x, newY);
        }
        else
        {
            motion.offset = new Vector2(motion.offset.x, groundY);
        }

        _wasGrounded = grounded;

        // Release the jump lock ONLY when the Jump clip truly finished
        if (animator != null && _jumpInProgress)
        {
            var s = animator.GetCurrentAnimatorStateInfo(0);
            if (s.IsName(jumpStateName) && s.normalizedTime >= 1f)
                _jumpInProgress = false;
        }

        // Animator run flag
        if (animator != null)
        {
            bool isRunning = alwaysRun ? grounded : (Mathf.Abs(_inputX) > runDeadzone && grounded);
            animator.SetBool(RunningBoolHash, isRunning);
        }
    }

    private void OnJumpPerformed()
    {
        _jumpRequested = true;
        _lastJumpPressedTime = Time.time;
    }

    private void OnJumpCanceled()
    {
        // Variable jump height: cut upward velocity if button released while rising
        if (_yVel > 0f) _yVel *= 0.5f;
    }

    private void DoJump()
    {
        _yVel = jumpSpeed;

        var motion = runner.motion;
        motion.offset = new Vector2(motion.offset.x, groundY + 0.0001f);

        if (animator != null)
        {
            animator.SetTrigger(JumpTriggerHash);
            _jumpInProgress = true; // block new jumps until Jump clip ends
        }

        // keep your audio intact
        GetComponent<JumpSFX>()?.OnJumpHappened();
    }
}
