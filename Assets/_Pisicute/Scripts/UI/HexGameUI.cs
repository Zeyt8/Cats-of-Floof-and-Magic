using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HexGameUI : MonoBehaviour
{
    [SerializeField] HexGrid grid;
    [SerializeField] InputHandler inputHandler;
    private HexCell currentCell;
    private UnitObject selectedUnit;

    private void OnEnable()
    {
        inputHandler.player.OnSelectCell.AddListener(DoSelection);
    }

    private void OnDisable()
    {
        inputHandler.player.OnSelectCell.RemoveListener(DoSelection);
    }

    private void Update()
    {
        DoPathfinding();
    }

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
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
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (cell != currentCell)
        {
            currentCell = cell;
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

        if (selectedUnit)
        {
            DoMove();
        }

        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell)
        {
            selectedUnit = currentCell.unit;
        }
    }

    private void DoPathfinding()
    {
        if (!UpdateCurrentCell()) return;
        if (currentCell && selectedUnit && selectedUnit.IsValidDestination(currentCell))
        {
            grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
        }
        else
        {
            grid.ClearPath();
        }
    }

    private void DoMove()
    {
        if (grid.HasPath)
        {
            selectedUnit.Travel(grid.GetPath());
            grid.ClearPath();
        }
    }
}
