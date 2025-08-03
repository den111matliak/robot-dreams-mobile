using UnityEngine;
using Dreamteck.Splines;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float strafeSpeed = 5f;
    private float targetOffset = 0f;
    private BasicRunner runner;

    private void Awake()
    {
        runner = GetComponent<BasicRunner>();
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        targetOffset += moveInput.x * strafeSpeed * Time.deltaTime;
    }

    private void Update()
    {
        if (runner != null)
        {
            var motion = runner.motion;
            motion.offset.x = targetOffset;
            runner.motion = motion;
        }
    }
}
