using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraInputHandler inputHandler;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private float speed;
    [SerializeField] private float orbitSpeed;
    [SerializeField] private float zoomScrollSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float zoomLowerClamp;
    [SerializeField] private float zoomUpperClamp;

    private bool isOrbiting;
    private float targetZoom;
    private float currentZoom;
    private float polar;
    private float elevation;
    private CinemachineTransposer transposer;
    private CinemachineHardLookAt hardLookAt;

    private void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        hardLookAt = virtualCamera.GetCinemachineComponent<CinemachineHardLookAt>();
        currentZoom = targetZoom = zoomUpperClamp;
        elevation = Mathf.PI / 3;
        polar = -Mathf.PI / 2;
    }

    private void OnEnable()
    {
        inputHandler.OnToggleOrbit.AddListener(ToggleOrbit);
    }

    private void OnDisable()
    {
        inputHandler.OnToggleOrbit.RemoveListener(ToggleOrbit);
    }

    private void Update()
    {
        transform.position += AdjustInputToFaceCamera(inputHandler.pan) * (Time.deltaTime * speed);

        targetZoom += inputHandler.zoom * zoomScrollSpeed;
        targetZoom = Mathf.Clamp(targetZoom, zoomLowerClamp, zoomUpperClamp);
        currentZoom = Mathf.MoveTowards(currentZoom, targetZoom, Mathf.Abs(currentZoom - targetZoom) / 10 * zoomSpeed * Time.deltaTime);
        
        if (isOrbiting)
        {
            polar += inputHandler.orbit.x * orbitSpeed * Time.deltaTime;
            elevation += inputHandler.orbit.y * orbitSpeed * Time.deltaTime;
            elevation = Mathf.Clamp(elevation, 0.1f, Mathf.PI / 2 - 0.01f);
        }

        transposer.m_FollowOffset = ToCartesian(polar, elevation, currentZoom);
    }

    private void ToggleOrbit(bool toggle)
    {
        isOrbiting = toggle;
        hardLookAt.enabled = toggle;
    }

    public Vector3 ToCartesian(float polar, float elevation, float radius)
    {
        float a = radius * Mathf.Cos(elevation);
        return new Vector3(
            a * Mathf.Cos(polar),
            radius * Mathf.Sin(elevation),
            a * Mathf.Sin(polar));
    }

    private Vector3 AdjustInputToFaceCamera(Vector3 moveInput)
    {
        float facing = Camera.main.transform.eulerAngles.y;
        return (Quaternion.Euler(0, facing, 0) * moveInput);
    }
}
