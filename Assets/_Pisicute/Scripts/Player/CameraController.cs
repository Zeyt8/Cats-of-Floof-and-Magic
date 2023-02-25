using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [SerializeField] private float _speed;
    [SerializeField] private float _orbitSpeed;
    [SerializeField] private float _zoomScrollSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomLowerClamp;
    [SerializeField] private float _zoomUpperClamp;

    private bool _isOrbiting;
    private float _targetZoom;
    private float _currentZoom;
    private float _polar;
    private float _elevation;
    private CinemachineTransposer _transposer;
    private CinemachineHardLookAt _hardLookAt;

    private void Awake()
    {
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _hardLookAt = _virtualCamera.GetCinemachineComponent<CinemachineHardLookAt>();
        _currentZoom = _targetZoom = _zoomUpperClamp;
        _elevation = Mathf.PI / 4;
        _polar = -Mathf.PI / 2;
    }

    private void OnEnable()
    {
        _inputHandler.Camera.OnToggleOrbit.AddListener(ToggleOrbit);
    }

    private void OnDisable()
    {
        _inputHandler.Camera.OnToggleOrbit.RemoveListener(ToggleOrbit);
    }

    private void Update()
    {
        transform.position += AdjustInputToFaceCamera(_inputHandler.Camera.Pan) * (Time.deltaTime * _speed);

        _targetZoom += _inputHandler.Camera.Zoom * _zoomScrollSpeed;
        _targetZoom = Mathf.Clamp(_targetZoom, _zoomLowerClamp, _zoomUpperClamp);
        _currentZoom = Mathf.MoveTowards(_currentZoom, _targetZoom, Mathf.Abs(_currentZoom - _targetZoom) / 10 * _zoomSpeed * Time.deltaTime);
        
        if (_isOrbiting)
        {
            _polar += _inputHandler.Camera.Orbit.x * _orbitSpeed * Time.deltaTime;
            _elevation += _inputHandler.Camera.Orbit.y * _orbitSpeed * Time.deltaTime;
            _elevation = Mathf.Clamp(_elevation, 0.1f, Mathf.PI / 2 - 0.01f);
        }

        _transposer.m_FollowOffset = ToCartesian(_polar, _elevation, _currentZoom);
    }

    private void ToggleOrbit(bool toggle)
    {
        _isOrbiting = toggle;
        _hardLookAt.enabled = toggle;
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
