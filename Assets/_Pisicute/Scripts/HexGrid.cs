using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour, ISaveableObject
{
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
    
    private void Awake()
    {
        HexMetrics.NoiseSource = _noiseSource;
        HexMetrics.InitializeHashGrid(_seed);

        CreateMap(_cellCountX, _cellCountZ);
    }

    private void OnEnable()
    {
        if (!HexMetrics.NoiseSource)
        {
            HexMetrics.NoiseSource = _noiseSource;
            HexMetrics.InitializeHashGrid(_seed);
        }
    }

    public bool CreateMap(int x, int z)
    {
        if (x <= 0 || x % HexMetrics.ChunkSizeX != 0 || z <= 0 || z % HexMetrics.ChunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }
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
    }

    public void Load(BinaryReader reader, int header = -1)
    {
        int x = reader.ReadInt32();
        int z = reader.ReadInt32();
        if (x != _cellCountX || z != _cellCountZ)
        {
            if (!CreateMap(x, z))
            {
                return;
            }
        }
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Load(reader);
        }
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].Refresh();
        }
    }

    public void FindPath(HexCell fromCell, HexCell toCell)
    {
        _searchFrontier.Clear();
        foreach (HexCell t in _cells)
        {
            t.Distance = int.MaxValue;
            t.DisableHighlight();
        }
        fromCell.EnableHighlight(Color.blue);
        toCell.EnableHighlight(Color.red);
        fromCell.Distance = 0;
        _searchFrontier.Enqueue(fromCell);
        while (_searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            if (current == toCell)
            {
                current = current.PathFrom;
                while (current != fromCell)
                {
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
                break;
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.IsUnderwater) continue;
                HexEdgeType edgeType = current.GetEdgeType(neighbor);
                if (edgeType == HexEdgeType.Cliff) continue;
                int distance = current.Distance;
                if (current.HasRoadThroughEdge(d))
                {
                    distance += 1;
                }
                else if (current.Walled != neighbor.Walled)
                {
                    continue;
                }
                else
                {
                    distance += edgeType == HexEdgeType.Flat ? 5 : 10;
                    distance += neighbor.UrbanLevel + neighbor.FarmLevel + neighbor.PlantLevel;
                }

                if (neighbor.Distance == int.MaxValue)
                {
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
    }
}
