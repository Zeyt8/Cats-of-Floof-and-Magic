using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour, ISaveableObject
{
    public UnitObject UnitPrefab;
    public bool HasPath => _currentPathExists;

    [SerializeField] private int _cellCountX = 4;
    [SerializeField] private int _cellCountZ = 3;
    [SerializeField] private int _seed;
    
    [SerializeField] private HexCell _cellPrefab;
    [SerializeField] private HexGridChunk _chunkPrefab;
    [SerializeField] private TextMeshProUGUI _cellLabelPrefab;
    [SerializeField] private Texture2D _noiseSource;
    private int _chunkCountX;
    private int _chunkCountZ;
    private HexCell[] _cells;
    private HexGridChunk[] _chunks;
    private HexCellPriorityQueue _searchFrontier = new HexCellPriorityQueue();
    private int _searchFrontierPhase;
    private HexCell _currentPathFrom, _currentPathTo;
    private bool _currentPathExists;
    private List<UnitObject> _units = new List<UnitObject>();
    private HexCellShaderData _cellShaderData;

    private void Awake()
    {
        HexMetrics.NoiseSource = _noiseSource;
        HexMetrics.InitializeHashGrid(_seed);
        _cellShaderData = GetComponent<HexCellShaderData>();

        CreateMap(_cellCountX, _cellCountZ);
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
        if (_chunks != null)
        {
            foreach (HexGridChunk t in _chunks)
            {
                Destroy(t.gameObject);
            }
        }
        _cellCountX = x;
        _cellCountZ = z;
        _chunkCountX = _cellCountX / HexMetrics.ChunkSizeX;
        _chunkCountZ = _cellCountZ / HexMetrics.ChunkSizeZ;
        _cellShaderData.Initialize(_cellCountX, _cellCountZ);

        CreateChunks();
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
        if (z < 0 || z >= _cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= _cellCountX)
        {
            return null;
        }
        return _cells[x + z * _cellCountX];
    }

    public void ShowUI(bool visible)
    {
        foreach (HexGridChunk t in _chunks)
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

    private void CreateChunks()
    {
        _chunks = new HexGridChunk[_chunkCountX * _chunkCountZ];

        for (int z = 0, i = 0; z < _chunkCountZ; z++)
        {
            for (int x = 0; x < _chunkCountX; x++)
            {
                HexGridChunk chunk = _chunks[i++] = Instantiate(_chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        _cells = new HexCell[_cellCountX * _cellCountZ];
        for (int z = 0, i = 0; z < _cellCountZ; z++)
        {
            for (int x = 0; x < _cellCountX; x++)
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

        HexCell cell = _cells[i] = Instantiate(_cellPrefab);
        cell.transform.localPosition = position;
        cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Index = i;
        cell.ShaderData = _cellShaderData;
        cell.IsExplorable = x > 0 && z > 0 && x < _cellCountX - 1 && z < _cellCountZ - 1;
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
        }
        if (z > 0)
        {
            if (z % 2 == 0)
            {
                cell.SetNeighbor(HexDirection.SE, _cells[i - _cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - _cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, _cells[i - _cellCountX]);
                if (x < _cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - _cellCountX + 1]);
                }
            }
        }

        // UI labels
        TextMeshProUGUI label = Instantiate(_cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        cell.UiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.ChunkSizeX;
        int chunkZ = z / HexMetrics.ChunkSizeZ;
        HexGridChunk chunk = _chunks[chunkX + chunkZ * _chunkCountX];

        int localX = x - chunkX * HexMetrics.ChunkSizeX;
        int localZ = z - chunkZ * HexMetrics.ChunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(_cellCountX);
        writer.Write(_cellCountZ);
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Save(writer);
        }

        writer.Write(_units.Count);
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        ClearPath();
        ClearUnits();
        int x = reader.ReadInt32();
        int z = reader.ReadInt32();
        if (x != _cellCountX || z != _cellCountZ)
        {
            if (!CreateMap(x, z))
            {
                return;
            }
        }
        bool originalImmediateMode = _cellShaderData.ImmediateMode;
        _cellShaderData.ImmediateMode = true;
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Load(reader, header);
        }
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].Refresh();
        }

        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            UnitObject unitObject = Instantiate(UnitPrefab);
            unitObject.Load(reader, header, grid);
        }
        _cellShaderData.ImmediateMode = originalImmediateMode;
    }

    public void FindPath(HexCell fromCell, HexCell toCell, UnitObject unit)
    {
        ClearPath();
        _currentPathFrom = fromCell;
        _currentPathTo = toCell;
        _currentPathExists = Search(fromCell, toCell, unit);
        ShowPath(unit.Speed);
    }

    private bool Search(HexCell fromCell, HexCell toCell, UnitObject unit)
    {
        int speed = unit.Speed;
        _searchFrontierPhase += 2;
        _searchFrontier.Clear();
        fromCell.SearchPhase = _searchFrontierPhase;
        fromCell.Distance = 0;
        _searchFrontier.Enqueue(fromCell);
        while (_searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            current.SearchPhase += 1;
            if (current == toCell)
            {
                return true;
            }
            int currentTurn = (current.Distance - 1) / speed;
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > _searchFrontierPhase) continue;
                if (!unit.IsValidDestination(neighbor)) continue;
                int moveCost = unit.GetMoveCost(current, neighbor, d);
                if (moveCost < 0) continue;
                
                int distance = current.Distance + moveCost;
                int turn = (distance - 1) / speed;
                if (turn > currentTurn)
                {
                    distance = turn * speed + moveCost;
                }

                if (neighbor.SearchPhase < _searchFrontierPhase)
                {
                    neighbor.SearchPhase = _searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic = neighbor.Coordinates.DistanceTo(toCell.Coordinates);
                    _searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    _searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return false;
    }

    private void ShowPath(int speed)
    {
        if (_currentPathExists)
        {
            HexCell current = _currentPathTo;
            while (current != _currentPathFrom)
            {
                int turn = (current.Distance - 1) / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }
        _currentPathFrom.EnableHighlight(Color.blue);
        _currentPathTo.EnableHighlight(Color.red);
    }

    public void ClearPath()
    {
        if (_currentPathExists)
        {
            HexCell current = _currentPathTo;
            while (current != _currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            _currentPathExists = false;
        }
        else if (_currentPathFrom)
        {
            _currentPathFrom.DisableHighlight();
            _currentPathTo.DisableHighlight();
        }
        _currentPathFrom = _currentPathTo = null;
    }

    public void AddUnit(UnitObject unit, HexCell location, float orientation)
    {
        _units.Add(unit);
        unit.Grid = this;
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(UnitObject unit)
    {
        _units.Remove(unit);
        unit.Die();
    }

    private void ClearUnits()
    {
        foreach (UnitObject u in _units)
        {
            u.Die();
        }
        _units.Clear();
    }

    public List<HexCell> GetPath()
    {
        if (!_currentPathExists)
        {
            return null;
        }
        List<HexCell> path = ListPool<HexCell>.Get();
        for (HexCell c = _currentPathTo; c != _currentPathFrom; c = c.PathFrom)
        {
            path.Add(c);
        }
        path.Add(_currentPathFrom);
        path.Reverse();
        return path;
    }

    private List<HexCell> GetVisibleCells(HexCell fromCell, int range, Func<HexCell, HexCell, bool> canSee)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();
        
        _searchFrontierPhase += 2;
        _searchFrontier.Clear();
        range += fromCell.ViewElevation;
        fromCell.SearchPhase = _searchFrontierPhase;
        fromCell.Distance = 0;
        _searchFrontier.Enqueue(fromCell);
        HexCoordinates fromCoordinates = fromCell.Coordinates;
        while (_searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            current.SearchPhase += 1;
            visibleCells.Add(current);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > _searchFrontierPhase || !neighbor.IsExplorable) continue;
                if (!canSee(current, neighbor)) continue;
                HexEdgeType edgeType = current.GetEdgeType(neighbor);

                int distance = current.Distance + 1;
                if (distance + neighbor.Elevation > range ||
                    distance > fromCoordinates.DistanceTo(neighbor.Coordinates)) continue;

                if (neighbor.SearchPhase < _searchFrontierPhase)
                {
                    neighbor.SearchPhase = _searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.SearchHeuristic = 0;
                    _searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    _searchFrontier.Change(neighbor, oldPriority);
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
