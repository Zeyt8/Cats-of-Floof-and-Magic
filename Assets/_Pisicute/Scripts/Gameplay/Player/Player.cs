using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    public Teams team;
    public int playerNumber;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private BuildingCollection buildingCollection;
    public Resources CurrentResources
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
    private HexCell lockedPath;

    public void Start()
    {
        CurrentResources = new Resources(10, 10, 0, 0, 0);
    }

    private void OnEnable()
    {
        inputHandler.OnSelectCell.AddListener(DoSelection);
        inputHandler.OnAltAction.AddListener(DoAlternateAction);
    }

    private void OnDisable()
    {
        inputHandler.OnSelectCell.RemoveListener(DoSelection);
        inputHandler.OnAltAction.RemoveListener(DoAlternateAction);
    }

    private bool UpdateCurrentCell()
    {
        HexCell cell = GetClickedCell();
        if (cell != currentCell)
        {
            if (currentCell)
            {
                currentCell.DisableHighlight();
            }
            currentCell = cell;
            if (currentCell)
            {
                currentCell.EnableHighlight(Color.white);
            }
            return true;
        }
        return false;
    }

    private HexCell GetClickedCell()
    {
        return GameManager.Instance.mapHexGrid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
    }

    private void DoSelection()
    {
        if (playerNumber != GameManager.Instance.currentPlayer) return;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        UpdateCurrentCell();
        if (currentCell == null) return;
        // build selected building
        if (buildingToBuild != BuildingTypes.None)
        {
            if (buildingCollection[buildingToBuild].resourceCost <= CurrentResources)
            {
                GameManager.Instance.mapHexGrid.AddBuilding(buildingToBuild, currentCell);
                CurrentResources -= buildingCollection[buildingToBuild].resourceCost;
            }
            buildingToBuild = BuildingTypes.None;
        }
        // if building on tile open building detail panel
        if (currentCell.Building != null)
        {
            GameManager.Instance.buildingDetails.Activate(currentCell.Building);
        }
        else
        {
            GameManager.Instance.buildingDetails.Deactivate();
        }
        // update selected unit
        selectedUnit = currentCell.unit;
        // if unit on tile open unit detail panel
        if (selectedUnit)
        {
            GameManager.Instance.unitDetails.Activate(selectedUnit);
        }
        else
        {
            GameManager.Instance.unitDetails.Deactivate();
        }
    }

    private void DoAlternateAction()
    {
        if (playerNumber != GameManager.Instance.currentPlayer) return;
        if (selectedUnit)
        {
            HexCell cell = GetClickedCell();
            if (lockedPath == cell)
            {
                DoMove();
            }
            else
            {
                DoPathfinding(cell);
            }
        }
    }

    private void DoPathfinding(HexCell cell)
    {
        if (cell && selectedUnit.IsValidDestination(cell))
        {
            GameManager.Instance.mapHexGrid.FindPath(selectedUnit.Location, cell, selectedUnit);
            lockedPath = cell;
        }
        else
        {
            GameManager.Instance.mapHexGrid.ClearPath();
        }
    }

    private void DoMove()
    {
        if (GameManager.Instance.mapHexGrid.HasPath)
        {
            selectedUnit.Travel(GameManager.Instance.mapHexGrid.GetPath());
            GameManager.Instance.mapHexGrid.ClearPath();
            lockedPath = null;
        }
    }
}