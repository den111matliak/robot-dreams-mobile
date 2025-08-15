using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputController : IDisposable, PlayerInputActions.IPlayerActions
{
    private readonly PlayerInputActions _actions;

    /// <summary>Поточний вектор руху</summary>
    public Vector2 CurrentMove { get; private set; }

    /// <summary>Чи затиснута кнопка стрибка зараз</summary>
    public bool IsJumpPressed { get; private set; }

    // Події для зовнішньої підписки (PlayerController)
    public event Action<Vector2> MovementReceived;
    public event Action JumpStarted;
    public event Action JumpPerformed;
    public event Action JumpCanceled;

    public InputController()
    {
        _actions = new PlayerInputActions();
        _actions.Player.SetCallbacks(this); // важливо: колбеки на мапу Player
        _actions.Enable();                  // увімкнути увесь asset
    }

    public void Dispose()
    {
        _actions.Player.RemoveCallbacks(this);
        _actions.Disable();
        (_actions as IDisposable)?.Dispose();
    }

    // ===== Реалізація інтерфейсу IPlayerActions =====

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CurrentMove = context.ReadValue<Vector2>();
            MovementReceived?.Invoke(CurrentMove);
        }
        else if (context.canceled)
        {
            CurrentMove = Vector2.zero;
            MovementReceived?.Invoke(CurrentMove);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsJumpPressed = true;
            JumpStarted?.Invoke();
        }
        else if (context.performed)
        {
            JumpPerformed?.Invoke();
        }
        else if (context.canceled)
        {
            IsJumpPressed = false;
            JumpCanceled?.Invoke();
        }
    }
}
