using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    [SerializeField] HexGrid grid;
    [SerializeField] PlayerInputHandler inputHandler;
    [SerializeField] BuildingCollection buildingCollection;
    [HideInInspector] public BuildingTypes buildingToBuild;

    private HexCell currentCell;
    private UnitObject selectedUnit;

    private void OnEnable()
    {
        inputHandler.OnSelectCell.AddListener(DoSelection);
    }

    private void OnDisable()
    {
        inputHandler.OnSelectCell.RemoveListener(DoSelection);
    }

    private void Update()
    {
        DoPathfinding();
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
        else
        {
            if (buildingToBuild != BuildingTypes.None)
            {
                // build
                currentCell.Building = buildingToBuild;
            }
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