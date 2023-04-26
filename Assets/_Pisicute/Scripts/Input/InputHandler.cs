using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputHandler", menuName = "Scriptable Objects/Input Handler")]
public class InputHandler : ScriptableObject, InputControlSchemes.IPlayerActions, InputControlSchemes.IMapEditorActions, InputControlSchemes.ICameraActions
{
    public PlayerInput player = new PlayerInput();
    public MapEditorInput mapEditor = new MapEditorInput();
    public CameraInput camera = new CameraInput();
    private InputControlSchemes inputControlSchemes;

    public class PlayerInput
    {
        public UnityEvent OnSelectCell = new UnityEvent();
    }

    public class MapEditorInput
    {
        public bool isEditing;
        public UnityEvent OnCreateUnit = new UnityEvent();
        public UnityEvent OnDestroyUnit = new UnityEvent();
    }

    public class CameraInput
    {
        public Vector3 pan;
        public Vector2 orbit;
        public float zoom;
        public UnityEvent<bool> OnToggleOrbit = new UnityEvent<bool>();
    }

    private void OnEnable()
    {
        inputControlSchemes = new InputControlSchemes();
        inputControlSchemes.Player.SetCallbacks(this);
        inputControlSchemes.MapEditor.SetCallbacks(this);
        inputControlSchemes.Camera.SetCallbacks(this);
        inputControlSchemes.Player.Enable();
        inputControlSchemes.MapEditor.Enable();
        inputControlSchemes.Camera.Enable();
    }

    #region Player Input
    void InputControlSchemes.IPlayerActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.OnSelectCell?.Invoke();
        }
    }

    #endregion

    #region Map Editor Input
    void InputControlSchemes.IMapEditorActions.OnSelectCell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mapEditor.isEditing = true;
        }
        else
        {
            mapEditor.isEditing = false;
        }
    }

    public void OnCreateUnit(InputAction.CallbackContext context)
    {
        mapEditor.OnCreateUnit?.Invoke();
    }

    public void OnDestroyUnit(InputAction.CallbackContext context)
    {
        mapEditor.OnDestroyUnit?.Invoke();
    }

    #endregion

    #region Camera Input
    void InputControlSchemes.ICameraActions.OnCameraPan(InputAction.CallbackContext context)
    {
        Vector2 pan = context.ReadValue<Vector2>();
        camera.pan = new Vector3(pan.x, 0, pan.y);
    }

    void InputControlSchemes.ICameraActions.OnCameraOrbit(InputAction.CallbackContext context)
    {
        camera.orbit = -context.ReadValue<Vector2>();
    }

    void InputControlSchemes.ICameraActions.OnZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        camera.zoom = -context.ReadValue<Vector2>().y;
    }

    void InputControlSchemes.ICameraActions.OnActivateOrbit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        camera.OnToggleOrbit?.Invoke(context.performed);
    }
    #endregion
}
