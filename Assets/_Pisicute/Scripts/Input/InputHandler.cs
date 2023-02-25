using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputHandler", menuName = "Scriptable Objects/Input Handler")]
public class InputHandler : ScriptableObject, InputControlSchemes.IPlayerActions, InputControlSchemes.IMapEditorActions, InputControlSchemes.ICameraActions
{
    public PlayerInput Player = new PlayerInput();
    public MapEditorInput MapEditor = new MapEditorInput();
    public CameraInput Camera = new CameraInput();
    private InputControlSchemes _inputControlSchemes;

    public class PlayerInput
    {
        public UnityEvent OnSelectCell = new UnityEvent();
    }

    public class MapEditorInput
    {
        public bool IsEditing;
    }

    public class CameraInput
    {
        public Vector3 Pan;
        public Vector2 Orbit;
        public float Zoom;
        public UnityEvent<bool> OnToggleOrbit = new UnityEvent<bool>();
    }

    private void OnEnable()
    {
        _inputControlSchemes = new InputControlSchemes();
        _inputControlSchemes.Player.SetCallbacks(this);
        _inputControlSchemes.MapEditor.SetCallbacks(this);
        _inputControlSchemes.Camera.SetCallbacks(this);
        _inputControlSchemes.Player.Enable();
        _inputControlSchemes.MapEditor.Enable();
        _inputControlSchemes.Camera.Enable();
    }

    #region Player Input
    void InputControlSchemes.IPlayerActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Player.OnSelectCell?.Invoke();
        }
    }
    #endregion

    #region Map Editor Input
    void InputControlSchemes.IMapEditorActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MapEditor.IsEditing = true;
        }
        else
        {
            MapEditor.IsEditing = false;
        }
    }
    #endregion

    #region Camera Input
    void InputControlSchemes.ICameraActions.OnCameraPan(InputAction.CallbackContext context)
    {
        Vector2 pan = context.ReadValue<Vector2>();
        Camera.Pan = new Vector3(pan.x, 0, pan.y);
    }

    void InputControlSchemes.ICameraActions.OnCameraOrbit(InputAction.CallbackContext context)
    {
        Camera.Orbit = -context.ReadValue<Vector2>();
    }

    void InputControlSchemes.ICameraActions.OnZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Camera.Zoom = -context.ReadValue<Vector2>().y;
    }

    void InputControlSchemes.ICameraActions.OnActivateOrbit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Camera.OnToggleOrbit?.Invoke(context.performed);
    }
    #endregion
}
