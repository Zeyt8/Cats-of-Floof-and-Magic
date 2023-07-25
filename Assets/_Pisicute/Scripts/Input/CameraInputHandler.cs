using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CameraInputHandler", menuName = "Scriptable Objects/Input Handlers/CameraInputHandler", order = 3)]
public class CameraInputHandler : InputHandler, InputControlSchemes.ICameraActions
{
    public Vector3 pan;
    public Vector2 orbit;
    public float zoom;
    public UnityEvent<bool> OnToggleOrbit = new UnityEvent<bool>();

    protected override void OnEnable()
    {
        base.OnEnable();
        inputControlSchemes.Camera.SetCallbacks(this);
    }

    void InputControlSchemes.ICameraActions.OnCameraPan(InputAction.CallbackContext context)
    {
        pan = context.ReadValue<Vector2>();
        pan = new Vector3(pan.x, 0, pan.y);
    }

    void InputControlSchemes.ICameraActions.OnCameraOrbit(InputAction.CallbackContext context)
    {
        orbit = -context.ReadValue<Vector2>();
    }

    void InputControlSchemes.ICameraActions.OnZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        zoom = -context.ReadValue<Vector2>().y;
    }

    void InputControlSchemes.ICameraActions.OnActivateOrbit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnToggleOrbit?.Invoke(context.performed);
    }
}
