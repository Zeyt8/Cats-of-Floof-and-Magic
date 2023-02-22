using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "InputHandler", menuName = "Scriptable Objects/Input Handler")]
public class InputHandler : ScriptableObject, InputControlSchemes.IPlayerActions, InputControlSchemes.IMapEditorActions
{
    public PlayerInput Player = new PlayerInput();
    public MapEditorInput MapEditor = new MapEditorInput();
    private InputControlSchemes _inputControlSchemes;

    public class PlayerInput
    {
        public UnityEvent SelectCell = new UnityEvent();
    }

    public class MapEditorInput
    {
        public UnityEvent SelectCell = new UnityEvent();
    }

    private void OnEnable()
    {
        _inputControlSchemes = new InputControlSchemes();
        _inputControlSchemes.Player.SetCallbacks(this);
        _inputControlSchemes.MapEditor.SetCallbacks(this);
        _inputControlSchemes.Player.Enable();
        _inputControlSchemes.MapEditor.Enable();
    }

    void InputControlSchemes.IPlayerActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Player.SelectCell?.Invoke();
        }
    }

    void InputControlSchemes.IMapEditorActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MapEditor.SelectCell?.Invoke();
        }
    }
}
