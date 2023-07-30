using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputHandler", menuName = "Scriptable Objects/Input Handlers/PlayerInputHandler", order = 1)]
public class PlayerInputHandler : InputHandler, InputControlSchemes.IPlayerActions
{
    public UnityEvent OnSelectCell = new UnityEvent();
    public UnityEvent OnAction = new UnityEvent();
    public UnityEvent OnAltAction = new UnityEvent();

    protected override void OnEnable()
    {
        base.OnEnable();
        inputControlSchemes.Player.SetCallbacks(this);
    }

    void InputControlSchemes.IPlayerActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSelectCell?.Invoke();
        }
    }

    void InputControlSchemes.IPlayerActions.OnAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAction?.Invoke();
        }
    }

    void InputControlSchemes.IPlayerActions.OnAltAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAltAction?.Invoke();
        }
    }
}
