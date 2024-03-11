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
            LevelManager.Instance.resourcesPanel.SetResourcesUI(currentResources, resourceGain);
        }
    }
    private Resources currentResources;
    public Resources ResourceGain
    {
        get => resourceGain;
        set
        {
            resourceGain = value;
            LevelManager.Instance.resourcesPanel.SetResourcesUI(currentResources, resourceGain);
        }
    }
    private Resources resourceGain = new Resources(0, 0, 0, 0, 0, 0);
    [HideInInspector] public BuildingTypes buildingToBuild;

    private HexCell currentCell;
    private UnitObject selectedUnit;
    private HexCell lockedPath;

    public delegate void Action<HexCell>(HexCell cell);
    Action<HexCell> onClickAction;
    Func<HexCell, bool> selectionCondition;
    private int delay;

    public void Start()
    {
        CurrentResources = new Resources(10, 10, 10, 0, 0, 0);
        playerNumber = NetworkPlayerUtils.GetPlayerIndex(NetworkHandler.PlayerId);
    }

    private void LateUpdate()
    {
        for (int i = leaders.Count - 1; i >= 0; i--)
        {
            if (leaders[i] == null)
            {
                leaders.RemoveAt(i);
            }
        }
        if (leaders.Count == 0 && delay > 1)
        {
            LoseServerRpc();
        }
        else
        {
            delay++;
        }
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
        if (selectionCondition == null)
        {
            onClickAction(null);
            return;
        }
        this.selectionCondition = selectionCondition;
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
            if (selectionCondition != null && !selectionCondition(currentCell))
            {
                onClickAction = null;
                selectionCondition = null;
                return;
            }
            onClickAction(currentCell);
            onClickAction = null;
            selectionCondition = null;
            foreach (HexCell cell in LevelManager.Instance.CurrentMap.cells)
            {
                cell.DisableHighlight();
            }
            if (!LevelManager.IsBattleActive)
            {
                SelectCell(currentCell);
            }
            return;
        }
        if (!LevelManager.IsBattleActive)
        {
            SelectionWorldMap(currentCell);
        }
        else
        {
            if (playerNumber == LevelManager.Instance.currentBattleMap.currentPlayer)
            {
                SelectionBattleMapServerRpc(currentCell.coordinates, playerNumber);
            }
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
        selectedUnit = cell.Unit;
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
        if (buildingToBuild != BuildingTypes.None && playerNumber == LevelManager.Instance.currentPlayer)
        {
            if (buildingCollection[buildingToBuild].resourceCost <= CurrentResources &&
                cell.Building == null &&
                cell.IsVisible
            )
            {
                BuildBuildingServerRpc(cell.coordinates, buildingToBuild, playerNumber);
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
    private void BuildBuildingServerRpc(HexCoordinates coordinates, BuildingTypes buildingToBuild, int owner)
    {
        BuildBuildingClientRpc(coordinates, buildingToBuild, owner);
    }

    [ClientRpc]
    private void BuildBuildingClientRpc(HexCoordinates coordinates, BuildingTypes buildingToBuild, int owner)
    {
        HexCell cell = LevelManager.Instance.mapHexGrid.GetCell(coordinates);
        Building building = cell.AddBuilding(buildingToBuild);
        if (building)
        {
            building.OnBuild(cell);
            building.ChangeOwner(owner);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectionBattleMapServerRpc(HexCoordinates coords, int player)
    {
        SelectionBattleMapClientRpc(coords, player);
    }

    [ClientRpc]
    private void SelectionBattleMapClientRpc(HexCoordinates coords, int player)
    {
        HexCell cell = LevelManager.Instance.CurrentMap.GetCell(coords);
        bool selected = LevelManager.Instance.currentBattleMap.SelectedCell(cell);
        if (player != playerNumber) return;
        if (selected)
        {
            selectedUnit = cell.Unit;
            if (selectedUnit)
            {
                BattleCanvas.Instance.ShowAbilities((Cat)selectedUnit);
            }
        }
        cell.EnableHighlight(HighlightType.Selection);
    }

    private void DoAlternateAction()
    {
        if (!LevelManager.IsBattleActive)
        {
            if (playerNumber != LevelManager.Instance.currentPlayer) return;
        }
        else
        {
            if (LevelManager.Instance.currentBattleMap.currentPlayer != playerNumber) return;
        }
        if (selectedUnit && selectedUnit.owner == playerNumber)
        {
            if (LevelManager.IsBattleActive && selectedUnit != LevelManager.Instance.currentBattleMap.CurrentCatTurn) return;
            HexCell cell = GetClickedCell();
            if (lockedPath == cell)
            {
                if (LevelManager.Instance.CurrentMap.HasPath)
                {
                    DoMoveServerRpc(LevelManager.Instance.CurrentMap.GetPath().Select(c => c.coordinates).ToArray(), selectedUnit.Location.coordinates);
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
    private void DoMoveServerRpc(HexCoordinates[] path, HexCoordinates unitLocation)
    {
        DoMoveClientRpc(path, unitLocation);
    }

    [ClientRpc]
    private void DoMoveClientRpc(HexCoordinates[] path, HexCoordinates unitLocation)
    {
        List<HexCell> cells = new List<HexCell>();
        foreach (HexCoordinates cell in path)
        {
            cells.Add(LevelManager.Instance.CurrentMap.GetCell(cell));
        }
        LevelManager.Instance.CurrentMap.GetCell(unitLocation).Unit.Travel(cells);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoseServerRpc()
    {
        GameManager.EndGame(2 - playerNumber);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddCatDataToLeaderServerRpc(CatData catData, HexCoordinates coords, int owner)
    {
        AddCatDataToLeaderClientRpc(catData, coords, owner);
    }

    [ClientRpc]
    private void AddCatDataToLeaderClientRpc(CatData catData, HexCoordinates coords, int owner)
    {
        foreach (UnitObject unit in LevelManager.Instance.mapHexGrid.GetCell(coords).units)
        {
            if (unit.owner == owner)
            {
                Leader leader = unit as Leader;
                leader.AddCatToArmy(catData);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddStatusEffectToUnitServerRpc(StatusEffect statusEffect, int map, HexCoordinates coords, int owner)
    {
        AddStatusEffectToUnitClientRpc(statusEffect, map, coords, owner);
    }

    [ClientRpc]
    private void AddStatusEffectToUnitClientRpc(StatusEffect statusEffect, int map, HexCoordinates coords, int owner)
    {
        HexGrid grid;
        if (map == -1)
        {
            grid = LevelManager.Instance.mapHexGrid;
        }
        else
        {
            grid = BattleManager.GetBattleMap(map).hexGrid;
        }
        foreach (UnitObject unit in grid.GetCell(coords).units)
        {
            if (unit.owner == owner)
            {
                Type t = Type.GetType(statusEffect.type.ToString());
                StatusEffect s = (StatusEffect)Activator.CreateInstance(t, statusEffect.duration, statusEffect.level, statusEffect.amount);
                unit.AddStatusEffect(s);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveStatusEffectFromUnitServerRpc(FixedString32Bytes type, int map, HexCoordinates coords, int owner)
    {
        RemoveStatusEffectFromUnitClientRpc(type, map, coords, owner);
    }

    [ClientRpc]
    private void RemoveStatusEffectFromUnitClientRpc(FixedString32Bytes type, int map, HexCoordinates coords, int owner)
    {
        HexGrid grid;
        if (map == -1)
        {
            grid = LevelManager.Instance.mapHexGrid;
        }
        else
        {
            grid = BattleManager.GetBattleMap(map).hexGrid;
        }
        foreach (UnitObject unit in grid.GetCell(coords).units)
        {
            if (unit.owner == owner)
            {
                unit.RemoveStatusEffect(type);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DealDamageToCatServerRpc(int damage, int map, HexCoordinates coords, int owner)
    {
        DealDamageToCatClientRpc(damage, map, coords, owner);
    }

    [ClientRpc]
    private void DealDamageToCatClientRpc(int damage, int map, HexCoordinates coords, int owner)
    {
        HexGrid grid = BattleManager.GetBattleMap(map).hexGrid;
        foreach (UnitObject unit in grid.GetCell(coords).units)
        {
            if (unit.owner == owner)
            {
                unit.TakeDamage(ref damage);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnOnBattleMapServerRpc(int map)
    {
        EndTurnBattleMapClientRpc(map);
    }

    [ClientRpc]
    private void EndTurnBattleMapClientRpc(int map)
    {
        BattleManager.GetBattleMap(map).EndTurn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportUnitServerRpc(HexCoordinates cell, HexCoordinates target)
    {
        TeleportUnitClientRpc(cell, target);
    }

    [ClientRpc]
    private void TeleportUnitClientRpc(HexCoordinates cell, HexCoordinates target)
    {
        UnitObject unit = LevelManager.Instance.mapHexGrid.GetCell(cell).Unit;
        if (unit)
        {
            unit.Location = LevelManager.Instance.mapHexGrid.GetCell(target);
        }
    }
}