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
    private OptionalToggle _riverMode;
    private bool _isDrag;
    private HexDirection _dragDirection;
    private HexCell _previousCell;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    // Start is called before the first frame update
    private void Awake()
    {
        SelectColor(-1);
    }

    private void Update()
    {
        if (_inputHandler.MapEditor.IsEditing && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else
        {
            _previousCell = null;
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(inputRay.origin, inputRay.direction * 100, Color.red);
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            HexCell currentCell = _hexGrid.GetCell(hit.point);
            if (_previousCell && _previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                _isDrag = false;
            }
            EditCells(currentCell);
            _previousCell = currentCell;
        }
        else
        {
            _previousCell = null;
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
        if (_riverMode == OptionalToggle.No)
        {
            cell.RemoveRiver();
        }
        else if (_isDrag && _riverMode == OptionalToggle.Yes)
        {
            HexCell otherCell = cell.GetNeighbor(_dragDirection.Opposite());
            if (otherCell)
            {
                otherCell.SetOutgoingRiver(_dragDirection);
            }
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

    private void ValidateDrag(HexCell currentCell)
    {
        for (_dragDirection = HexDirection.NE; _dragDirection <= HexDirection.NW; _dragDirection++)
        {
            if (_previousCell.GetNeighbor(_dragDirection) == currentCell)
            {
                _isDrag = true;
                return;
            }
        }
        _isDrag = false;
    }

    #region UI
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

    public void SetRiverMode(int mode)
    {
        _riverMode = (OptionalToggle)mode;
    }
    #endregion
}
