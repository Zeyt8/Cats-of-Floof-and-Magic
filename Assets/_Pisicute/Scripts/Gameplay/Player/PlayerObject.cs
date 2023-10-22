using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.Collections;

public class PlayerObject : NetworkSingleton<PlayerObject>
{
    public int team;
    public int playerNumber;
    public List<Leader> leaders = new List<Leader>();
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private BuildingCollection buildingCollection;
    public Resources CurrentResources
    {
        get => currentResources;
        set
        {
            currentResources = value;
            LevelManager.Instance.resourcesPanel.SetResourcesUI(currentResources);
        }
    }
    private Resources currentResources;
    [HideInInspector] public BuildingTypes buildingToBuild;

    private HexCell currentCell;
    private UnitObject selectedUnit;
    private HexCell lockedPath;

    public delegate void Action<HexCell>(HexCell cell);
    Action<HexCell> onClickAction;

    public void Start()
    {
        CurrentResources = new Resources(10, 10, 10, 0, 0, 0);
        playerNumber = NetworkPlayerUtils.GetPlayerIndex(NetworkHandler.PlayerId);
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

    public void InitiateSelectCellForEffect(Func<HexCell, bool> selectionCondition, Action<HexCell> onClickAction)
    {
        this.onClickAction = onClickAction;
        foreach (HexCell cell in LevelManager.Instance.CurrentMap.cells)
        {
            if (selectionCondition(cell))
            {
                cell.EnableHighlight(HighlightType.Path);
            }
            else
            {
                cell.DisableHighlight();
            }
        }
    }

    private void DoSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        LevelManager.Instance.CurrentMap.ClearPath();
        UpdateCurrentCell();
        if (currentCell == null) return;
        if (onClickAction != null)
        {
            onClickAction(currentCell);
            onClickAction = null;
            foreach (HexCell cell in LevelManager.Instance.CurrentMap.cells)
            {
                cell.DisableHighlight();
            }
            SelectCell(currentCell);
            return;
        }
        if (LevelManager.Instance.currentBattleMap == null && playerNumber == LevelManager.Instance.currentPlayer)
        {
            SelectionWorldMap(currentCell);
        }
        else if (playerNumber == LevelManager.Instance.currentBattleMap.currentPlayer)
        {
            SelectionBattleMap(currentCell);
        }
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
        return LevelManager.Instance.CurrentMap.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
    }

    public void SelectCell(HexCell cell)
    {
        // if building on tile open building detail panel
        if (cell.Building != null)
        {
            LevelManager.Instance.buildingDetails.Activate(cell.Building);
        }
        else
        {
            LevelManager.Instance.buildingDetails.Deactivate();
        }
        // update selected unit
        selectedUnit = cell.units.Count > 0 ? cell.units[0] : null;
        // if unit on tile open unit detail panel
        if (selectedUnit)
        {
            LevelManager.Instance.unitDetails.Activate(cell, (Leader)selectedUnit);
        }
        else
        {
            LevelManager.Instance.unitDetails.Deactivate();
        }
        cell.EnableHighlight(HighlightType.Selection);
    }

    private void SelectionWorldMap(HexCell cell)
    {
        // build selected building
        if (buildingToBuild != BuildingTypes.None)
        {
            if (buildingCollection[buildingToBuild].resourceCost <= CurrentResources &&
                cell.Building == null &&
                cell.IsVisible
            )
            {
                BuildBuildingServerRpc(cell.coordinates, buildingToBuild);
                if (cell.Building.type != BuildingTypes.None)
                {
                    CurrentResources -= buildingCollection[buildingToBuild].resourceCost;
                }
            }
            buildingToBuild = BuildingTypes.None;
        }
        SelectCell(cell);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BuildBuildingServerRpc(HexCoordinates coordinates, BuildingTypes buildingToBuild)
    {
        BuildBuildingClientRpc(coordinates, buildingToBuild);
    }

    [ClientRpc]
    private void BuildBuildingClientRpc(HexCoordinates coordinates, BuildingTypes buildingToBuild)
    {
        HexCell cell = LevelManager.Instance.mapHexGrid.GetCell(coordinates);
        Building building = cell.AddBuilding(buildingToBuild);
        if (building)
        {
            building.OnBuild(cell);
        }
    }

    private void SelectionBattleMap(HexCell cell)
    {
        if (LevelManager.Instance.currentBattleMap.SelectedCell(cell))
        {
            selectedUnit = cell.units.Count > 0 ? cell.units[0] : null;
        }
        cell.EnableHighlight(HighlightType.Selection);
    }

    private void DoAlternateAction()
    {
        if (LevelManager.Instance.currentBattleMap == null)
        {
            if (playerNumber != LevelManager.Instance.currentPlayer) return;
        }
        else
        {
            if (LevelManager.Instance.currentBattleMap.currentPlayer != playerNumber) return;
        }
        if (selectedUnit && selectedUnit.owner == playerNumber)
        {
            HexCell cell = GetClickedCell();
            if (lockedPath == cell)
            {
                if (LevelManager.Instance.CurrentMap.HasPath)
                {
                    DoMoveServerRpc(LevelManager.Instance.CurrentMap.GetPath().Select(c => c.coordinates).ToArray());
                    LevelManager.Instance.CurrentMap.ClearPath();
                    lockedPath = null;
                }
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
            LevelManager.Instance.CurrentMap.FindPath(selectedUnit.Location, cell, selectedUnit);
            lockedPath = cell;
        }
        else
        {
            LevelManager.Instance.CurrentMap.ClearPath();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoMoveServerRpc(HexCoordinates[] path)
    {
        DoMoveClientRpc(path);
    }

    [ClientRpc]
    private void DoMoveClientRpc(HexCoordinates[] path)
    {
        List<HexCell> cells = new List<HexCell>();
        foreach (HexCoordinates cell in path)
        {
            cells.Add(LevelManager.Instance.CurrentMap.GetCell(cell));
        }
        selectedUnit.Travel(cells);
    }
}