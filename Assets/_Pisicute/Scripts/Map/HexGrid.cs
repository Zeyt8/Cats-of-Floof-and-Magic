using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour, ISaveableObject
{
    public UnitObject unitPrefab;
    public BuildingCollection allBuildings;
    public bool HasPath => currentPathExists;

    public int cellCountX = 4;
    public int cellCountZ = 3;
    [SerializeField] private int seed;
    
    [SerializeField] private HexCell cellPrefab;
    [SerializeField] private HexGridChunk chunkPrefab;
    [SerializeField] private TextMeshProUGUI cellLabelPrefab;
    [SerializeField] private Texture2D noiseSource;
    private int chunkCountX;
    private int chunkCountZ;
    public HexCell[] cells;
    private HexGridChunk[] chunks;
    private HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
    private int searchFrontierPhase;
    private HexCell currentPathFrom, currentPathTo;
    private bool currentPathExists;
    private List<UnitObject> units = new List<UnitObject>();
    private List<Building> buildings = new List<Building>();
    private HexCellShaderData cellShaderData;

    private void Awake()
    {
        HexMetrics.NoiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        cellShaderData = GetComponent<HexCellShaderData>();
    }

    public bool CreateMap(int x, int z)
    {
        if (x <= 0 || x % HexMetrics.ChunkSizeX != 0 || z <= 0 || z % HexMetrics.ChunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }

        ClearPath();
        ClearUnits();
        if (chunks != null)
        {
            foreach (HexGridChunk t in chunks)
            {
                Destroy(t.gameObject);
            }
        }
        cellCountX = x;
        cellCountZ = z;
        chunkCountX = cellCountX / HexMetrics.ChunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.ChunkSizeZ;

        CreateChunks();
        cellShaderData.Initialize(cellCountX, cellCountZ);
        CreateCells();

        return true;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        return GetCell(coordinates);
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    public void ShowUI(bool visible)
    {
        if (chunks == null) return;
        foreach (HexGridChunk t in chunks)
        {
            t.ShowUI(visible);
        }
    }

    public HexCell GetCell(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return GetCell(hit.point);
        }
        return null;
    }

    public HexCell GetCell(int xOffset, int zOffset)
    {
        return cells[xOffset + zOffset * cellCountX];
    }

    public HexCell GetCell(int cellIndex)
    {
        return cells[cellIndex];
    }

    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];
        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.OuterRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.index = i;
        cell.shaderData = cellShaderData;
        cell.isExplorable = x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if (z % 2 == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        // UI labels
        TextMeshProUGUI label = Instantiate(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.ChunkSizeX;
        int chunkZ = z / HexMetrics.ChunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.ChunkSizeX;
        int localZ = z - chunkZ * HexMetrics.ChunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }

        writer.Write(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Save(writer);
        }

        writer.Write(buildings.Count);
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        ClearPath();
        ClearUnits();
        int x = reader.ReadInt32();
        int z = reader.ReadInt32();
        if (x != cellCountX || z != cellCountZ)
        {
            if (!CreateMap(x, z))
            {
                return;
            }
        }
        bool originalImmediateMode = cellShaderData.immediateMode;
        cellShaderData.immediateMode = true;
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader, header);
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }

        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            UnitObject unitObject = Instantiate(unitPrefab);
            unitObject.Load(reader, header, grid);
        }
        int buildingCount = reader.ReadInt32();
        for (int i = 0; i < buildingCount; i++)
        {
            BuildingTypes type = (BuildingTypes)reader.ReadInt32();
            Building buildingObject = Instantiate(allBuildings[type]);
            buildingObject.Load(reader, header, grid);
        }
        cellShaderData.immediateMode = originalImmediateMode;
    }

    public void FindPath(HexCell fromCell, HexCell toCell, UnitObject unit)
    {
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, unit);
        ShowPath(unit.Speed);
    }

    private bool Search(HexCell fromCell, HexCell toCell, UnitObject unit)
    {
        int speed = unit.Speed;
        searchFrontierPhase += 2;
        searchFrontier.Clear();
        fromCell.searchPhase = searchFrontierPhase;
        fromCell.distance = 0;
        searchFrontier.Enqueue(fromCell);
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.searchPhase += 1;
            if (current == toCell)
            {
                return true;
            }
            int currentTurn = (current.distance - 1) / speed;
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.searchPhase > searchFrontierPhase) continue;
                if (!unit.IsValidDestination(neighbor)) continue;
                int moveCost = unit.GetMoveCost(current, neighbor, d);
                if (moveCost < 0) continue;
                
                int distance = current.distance + moveCost;
                int turn = (distance - 1) / speed;
                if (turn > currentTurn)
                {
                    distance = turn * speed + moveCost;
                }

                if (neighbor.searchPhase < searchFrontierPhase)
                {
                    neighbor.searchPhase = searchFrontierPhase;
                    neighbor.distance = distance;
                    neighbor.pathFrom = current;
                    neighbor.searchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.distance = distance;
                    neighbor.pathFrom = current;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return false;
    }

    private void ShowPath(int speed)
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                int turn = (current.distance - 1) / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.gray);
                current = current.pathFrom;
            }
        }
        currentPathFrom.EnableHighlight(Color.blue);
        currentPathTo.EnableHighlight(Color.red);
    }

    public void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.pathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
    }

    public void AddBuilding(Building building, HexCell location)
    {
        if (location.Building != null) return;
        buildings.Add(building);
        building.grid = this;
        building.transform.SetParent(transform, false);
        building.Location = location;
        location.Building = building;
    }
    public void AddBuilding(BuildingTypes buildingType, HexCell location)
    {
        if (location.Building != null) return;
        Building building = Instantiate(allBuildings[buildingType]);
        AddBuilding(building, location);
    }

    public void RemoveBuilding(Building building)
    {
        buildings.Remove(building);
    }

    public void AddUnit(UnitObject unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.grid = this;
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(UnitObject unit)
    {
        units.Remove(unit);
        unit.Die();
    }

    private void ClearUnits()
    {
        foreach (UnitObject u in units)
        {
            u.Die();
        }
        units.Clear();
    }

    public List<HexCell> GetPath()
    {
        if (!currentPathExists)
        {
            return null;
        }
        List<HexCell> path = ListPool<HexCell>.Get();
        for (HexCell c = currentPathTo; c != currentPathFrom; c = c.pathFrom)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();
        return path;
    }

    private List<HexCell> GetVisibleCells(HexCell fromCell, int range, Func<HexCell, HexCell, bool> canSee)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();
        
        searchFrontierPhase += 2;
        searchFrontier.Clear();
        range += fromCell.ViewElevation;
        fromCell.searchPhase = searchFrontierPhase;
        fromCell.distance = 0;
        searchFrontier.Enqueue(fromCell);
        HexCoordinates fromCoordinates = fromCell.coordinates;
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.searchPhase += 1;
            visibleCells.Add(current);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.searchPhase > searchFrontierPhase || !neighbor.isExplorable) continue;
                if (!canSee(current, neighbor)) continue;
                HexEdgeType edgeType = current.GetEdgeType(neighbor);

                int distance = current.distance + 1;
                if (distance + neighbor.Elevation > range ||
                    distance > fromCoordinates.DistanceTo(neighbor.coordinates)) continue;

                if (neighbor.searchPhase < searchFrontierPhase)
                {
                    neighbor.searchPhase = searchFrontierPhase;
                    neighbor.distance = distance;
                    neighbor.searchHeuristic = 0;
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.distance = distance;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return visibleCells;
    }

    public void IncreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range, (c1, c2) => true);
        foreach (var t in cells)
        {
            t.IncreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range, (c1, c2) => true);
        foreach (var t in cells)
        {
            t.DecreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }
}
