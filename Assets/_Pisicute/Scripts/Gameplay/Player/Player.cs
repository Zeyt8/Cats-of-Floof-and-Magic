using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    [SerializeField] private HexGrid grid;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private BuildingCollection buildingCollection;
    private Resources CurrentResources
    {
        get => currentResources;
        set
        {
            currentResources = value;
            GameManager.Instance.resourcesPanel.SetResourcesUI(currentResources);
        }
    }
    private Resources currentResources;
    [HideInInspector] public BuildingTypes buildingToBuild;

    private HexCell currentCell;
    private UnitObject selectedUnit;

    public void Start()
    {
        CurrentResources = new Resources(10, 10, 0, 0);
    }

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
        if (selectedUnit != null)
        {
            DoPathfinding();
        }
    }

    private bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (cell != currentCell)
        {
            if (currentCell != null)
            {
                currentCell.DisableHighlight();
            }
            currentCell = cell;
            if (currentCell != null)
            {
                currentCell.EnableHighlight(Color.white);
            }
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
        UpdateCurrentCell();
        if (currentCell == null) return;
        if (currentCell.Building != BuildingTypes.None)
        {
            GameManager.Instance.buildingDetails.Activate(buildingCollection[currentCell.Building]);
        }
        else
        {
            GameManager.Instance.buildingDetails.Deactivate();
        }
        if (selectedUnit)
        {
            DoMove();
            selectedUnit = null;
        }
        else if (buildingToBuild != BuildingTypes.None)
        {
            if (buildingCollection[buildingToBuild].resourceCost <= CurrentResources)
            {
                currentCell.Building = buildingToBuild;
                CurrentResources -= buildingCollection[buildingToBuild].resourceCost;
            }
            buildingToBuild = BuildingTypes.None;
        }
        else
        {
            selectedUnit = currentCell.unit;
        }
        grid.ClearPath();
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