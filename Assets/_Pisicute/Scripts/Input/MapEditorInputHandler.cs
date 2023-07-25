using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "MapEditorInputHandler", menuName = "Scriptable Objects/Input Handlers/MapEditorInputHandler", order = 2)]
public class MapEditorInputHandler : InputHandler, InputControlSchemes.IMapEditorActions
{
    public bool isEditing;
    public UnityEvent OnCreateUnit = new UnityEvent();
    public UnityEvent OnDestroyUnit = new UnityEvent();
    
    protected override void OnEnable()
    {
        base.OnEnable();
        inputControlSchemes.MapEditor.SetCallbacks(this);
    }

    void InputControlSchemes.IMapEditorActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isEditing = true;
        }
        else
        {
            isEditing = false;
        }
    }

    void InputControlSchemes.IMapEditorActions.OnCreateUnit(InputAction.CallbackContext context)
    {
        OnCreateUnit?.Invoke();
    }

    void InputControlSchemes.IMapEditorActions.OnDestroyUnit(InputAction.CallbackContext context)
    {
        OnDestroyUnit?.Invoke();
    }
}
