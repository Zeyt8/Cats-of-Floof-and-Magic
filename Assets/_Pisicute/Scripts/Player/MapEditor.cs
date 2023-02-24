using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapEditor : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private HexGrid _hexGrid;
    [SerializeField] private Color[] _colors;
    private Color _activeColor;
    private int _activeElevation;

    // Start is called before the first frame update
    private void Awake()
    {
        SelectColor(0);
    }

    private void OnEnable()
    {
        _inputHandler.MapEditor.SelectCell.AddListener(HandleInput);
    }

    private void OnDisable()
    {
        _inputHandler.MapEditor.SelectCell.RemoveListener(HandleInput);
    }

    private void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            EditCell(_hexGrid.GetCell(hit.point));
        }
    }

    private void EditCell(HexCell cell)
    {
        cell.Color = _activeColor;
        cell.Elevation = _activeElevation;
        _hexGrid.Refresh();
    }

    public void SelectColor(int index)
    {
        _activeColor = _colors[index];
    }

    public void SetElevation(float elevation)
    {
        _activeElevation = (int)elevation;
    }
}
