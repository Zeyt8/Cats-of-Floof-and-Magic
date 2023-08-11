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
    private BattleMap currentBattleMap;

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

    public void GoToBattleMap(BattleMap map)
    {
        currentBattleMap = map;
        map.SetBattleActive(true);
    }

    public void GoToWorldMap()
    {
        currentBattleMap.SetBattleActive(false);
        currentBattleMap = null;
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
            return true;
        }
        return false;
    }

    private HexCell GetClickedCell()
    {
        if (currentBattleMap == null)
        {
            return GameManager.Instance.mapHexGrid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        }
        else
        {
            return currentBattleMap.hexGrid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        }
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
        if (currentBattleMap == null)
        {
            SelectionWorldMap(currentCell);
        }
        else
        {
            SelectionBattleMap(currentCell);
        }
    }

    private void SelectionWorldMap(HexCell cell)
    {
        // build selected building
        if (buildingToBuild != BuildingTypes.None)
        {
            if (buildingCollection[buildingToBuild].resourceCost <= CurrentResources)
            {
                GameManager.Instance.mapHexGrid.AddBuilding(buildingToBuild, cell);
                CurrentResources -= buildingCollection[buildingToBuild].resourceCost;
            }
            buildingToBuild = BuildingTypes.None;
        }
        SelectCell(cell);
    }

    public void SelectCell(HexCell cell)
    {
        // if building on tile open building detail panel
        if (cell.Building != null)
        {
            GameManager.Instance.buildingDetails.Activate(cell.Building);
        }
        else
        {
            GameManager.Instance.buildingDetails.Deactivate();
        }
        // update selected unit
        selectedUnit = cell.units.Count > 0 ? cell.units[0] : null;
        // if unit on tile open unit detail panel
        if (selectedUnit)
        {
            GameManager.Instance.unitDetails.Activate(cell, selectedUnit);
        }
        else
        {
            GameManager.Instance.unitDetails.Deactivate();
        }
        cell.EnableHighlight(HighlightType.Selection);
    }

    private void SelectionBattleMap(HexCell cell)
    {
        currentBattleMap.SelectedCell(cell);
        selectedUnit = cell.units.Count > 0 ? cell.units[0] : null;
        cell.EnableHighlight(HighlightType.Selection);
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
        if (selectedUnit.IsMoving) return;
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