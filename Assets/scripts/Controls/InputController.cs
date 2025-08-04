using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputController : IDisposable, PlayerInputActions.IPlayerActions
{
    private readonly PlayerInputActions _actions;

    // Подія, на яку підпишеться PlayerController
    public event Action<Vector2> MovementReceived;

    public InputController()
    {
        _actions = new PlayerInputActions();
        _actions.Enable();
        _actions.Player.SetCallbacks(this); // важливо!
    }

    public void Dispose()
    {
        _actions.Player.RemoveCallbacks(this);
        _actions.Disable();
    }

    // Callback із згенерованого інтерфейсу
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            MovementReceived?.Invoke(context.ReadValue<Vector2>());
    }
}
