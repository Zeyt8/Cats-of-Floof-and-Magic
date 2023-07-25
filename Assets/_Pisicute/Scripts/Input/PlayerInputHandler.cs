using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputHandler", menuName = "Scriptable Objects/Input Handlers/PlayerInputHandler", order = 1)]
public class PlayerInputHandler : InputHandler, InputControlSchemes.IPlayerActions
{
    public UnityEvent OnSelectCell = new UnityEvent();

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
}
