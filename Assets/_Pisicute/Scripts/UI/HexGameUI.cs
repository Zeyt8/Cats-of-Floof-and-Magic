using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HexGameUI : MonoBehaviour
{
    [SerializeField] HexGrid _grid;
    [SerializeField] InputHandler _inputHandler;
    private HexCell _currentCell;
    private UnitObject _selectedUnit;

    private void OnEnable()
    {
        _inputHandler.Player.OnSelectCell.AddListener(DoSelection);
    }

    private void OnDisable()
    {
        _inputHandler.Player.OnSelectCell.RemoveListener(DoSelection);
    }

    private void Update()
    {
        DoPathfinding();
    }

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        _grid.ShowUI(!toggle);
        _grid.ClearPath();
        if (toggle)
        {
            Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
        }
        else
        {
            Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
        }
    }

    private bool UpdateCurrentCell()
    {
        HexCell cell = _grid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (cell != _currentCell)
        {
            _currentCell = cell;
            return true;
        }
        return false;
    }

    private void DoSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (_selectedUnit)
        {
            DoMove();
        }

        _grid.ClearPath();
        UpdateCurrentCell();
        if (_currentCell)
        {
            _selectedUnit = _currentCell.Unit;
        }
    }

    private void DoPathfinding()
    {
        if (!UpdateCurrentCell()) return;
        if (_currentCell && _selectedUnit && _selectedUnit.IsValidDestination(_currentCell))
        {
            _grid.FindPath(_selectedUnit.Location, _currentCell, _selectedUnit);
        }
        else
        {
            _grid.ClearPath();
        }
    }

    private void DoMove()
    {
        if (_grid.HasPath)
        {
            _selectedUnit.Travel(_grid.GetPath());
            _grid.ClearPath();
        }
    }
}
