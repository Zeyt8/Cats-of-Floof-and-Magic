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
    private bool _applyColor;
    private bool _applyElevation;
    private int _brushSize;

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
            EditCells(_hexGrid.GetCell(hit.point));
        }
    }

    private void EditCell(HexCell cell)
    {
        if (!cell) return;
        if (_applyColor)
        {
            cell.Color = _activeColor;
        }
        if (_applyElevation)
        {
            cell.Elevation = _activeElevation;
        }
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.Coordinates.X;
        int centerZ = center.Coordinates.Z;
        for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + _brushSize; x++)
            {
                EditCell(_hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - _brushSize; x <= centerX + r; x++)
            {
                EditCell(_hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    public void SelectColor(int index)
    {
        _applyColor = index >= 0;
        if (_applyColor)
        {
            _activeColor = _colors[index];
        }
    }

    public void SetElevation(float elevation)
    {
        _activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        _applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        _brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        _hexGrid.ShowUI(visible);
    }
}
