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
    [SerializeField] private float jumpSpeed = 10f;     // вище/швидше стрибок
    [SerializeField] private float gravity = -40f;      // швидше падіння
    [SerializeField] private float groundY = 0f;
    [SerializeField] private float groundSnap = 0.01f;

    [Header("Jump UX (anti-spam)")]
    [SerializeField] private float coyoteTime = 0.10f;      // час після сходу із землі, коли ще можна стрибнути
    [SerializeField] private float jumpBufferTime = 0.10f;  // буфер натискання до моменту, коли стрибок стане можливим
    [SerializeField] private float groundedStableTime = 0.05f; // скільки потрібно побути на землі перед наступним стрибком

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool alwaysRun = true;     // для раннера

    private const string RunningBool = "isRunning";
    private const string JumpTrigger = "isJumping";     // ТРИГЕР у Animator
    private static readonly int RunningBoolHash = Animator.StringToHash(RunningBool);
    private static readonly int JumpTriggerHash = Animator.StringToHash(JumpTrigger);

    private InputController _input;
    private float _inputX;
    private float _yVel;

    // таймери/флаги для анти-спаму
    private bool _jumpRequested;            // натиснуто пробіл (збережено у буфер)
    private float _lastJumpPressedTime;     // коли востаннє натиснули пробіл
    private float _lastGroundedTime;        // коли востаннє були на землі
    private float _groundedEnteredTime;     // коли стали на землю і тримаємось стабільно
    private bool _wasGrounded;

    private void Awake()
    {
        if (runner == null) runner = GetComponent<Runner>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;

        _input = new InputController();
        _input.MovementReceived += OnMovementReceived;
        _input.JumpPerformed += OnJumpPerformed;  // натискання пробілу
        _input.JumpCanceled += OnJumpCanceled;   // відпускання пробілу
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

        // ---------- Горизонталь ----------
        float targetX = Mathf.Clamp(_inputX * maxOffset, -maxOffset, maxOffset);
        motion.offset = new Vector2(
            Mathf.MoveTowards(motion.offset.x, targetX, strafeSpeed * Time.deltaTime),
            motion.offset.y
        );

        // ---------- Перевірка землі (до інтеграції Y) ----------
        bool grounded = (motion.offset.y <= groundY + groundSnap) && _yVel <= 0f;
        float now = Time.time;

        if (grounded)
        {
            _lastGroundedTime = now;
            if (!_wasGrounded) _groundedEnteredTime = now; // початок стабільного стояння
        }

        // ---------- Виконання буферизованого стрибка ----------
        // Умови дозволу стрибка:
        // 1) або стабільно стоїмо на землі достатній час,
        // 2) або в межах "coyote time" після сходу із землі
        bool canJumpNow =
            (grounded && (now - _groundedEnteredTime) >= groundedStableTime)
            || (!grounded && (now - _lastGroundedTime) <= coyoteTime);

        // якщо є запит на стрибок і він ще в буфері — використати його, коли можна
        if (_jumpRequested && (now - _lastJumpPressedTime) <= jumpBufferTime && canJumpNow)
        {
            DoJump();
            _jumpRequested = false; // поглинаємо буфер
            grounded = false;       // вже не на землі
        }
        else if ((now - _lastJumpPressedTime) > jumpBufferTime)
        {
            // вичерпався буфер — скинути запит
            _jumpRequested = false;
        }

        // ---------- Вертикаль (інтеграція) ----------
        if (!grounded)
        {
            _yVel += gravity * Time.deltaTime;
            float newY = motion.offset.y + _yVel * Time.deltaTime;
            if (newY < groundY)
            {
                newY = groundY;
                _yVel = 0f;
                grounded = true;
                _groundedEnteredTime = now; // стали на землю
            }
            motion.offset = new Vector2(motion.offset.x, newY);
        }
        else
        {
            motion.offset = new Vector2(motion.offset.x, groundY);
        }

        _wasGrounded = grounded;

        // ---------- Анімація ----------
        if (animator != null)
        {
            bool isRunning = alwaysRun ? grounded : (Mathf.Abs(_inputX) > runDeadzone && grounded);
            animator.SetBool(RunningBoolHash, isRunning);
            // Jump — через Trigger у DoJump()
        }
    }

    // ===== Події інпуту =====
    private void OnJumpPerformed()
    {
        _jumpRequested = true;
        _lastJumpPressedTime = Time.time;
    }

    private void OnJumpCanceled()
    {
        // обрізання висоти, якщо відпустив під час підйому
        if (_yVel > 0f) _yVel *= 0.5f;
    }

    // ===== Реальний запуск стрибка =====
    private void DoJump()
    {
        _yVel = jumpSpeed;

        var motion = runner.motion; // беремо прямо тут
        motion.offset = new Vector2(motion.offset.x, groundY + 0.0001f); // легкий відрив

        if (animator != null)
            animator.SetTrigger(JumpTriggerHash);
    }

}
