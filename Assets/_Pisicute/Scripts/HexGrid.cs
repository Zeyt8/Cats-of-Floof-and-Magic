using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private int _chunkCountX = 4;
    [SerializeField] private int _chunckCountZ = 3;
    [SerializeField] private int _seed;
    
    [SerializeField] private HexCell _cellPrefab;
    [SerializeField] private HexGridChunk _chunkPrefab;
    [SerializeField] private TextMeshProUGUI _cellLabelPrefab;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.cyan;
    [SerializeField] private Texture2D _noiseSource;
    private int _cellCountX;
    private int _cellCountZ;
    private HexCell[] _cells;
    private HexGridChunk[] _chunks;
    
    private void Awake()
    {
        HexMetrics.NoiseSource = _noiseSource;
        HexMetrics.InitializeHashGrid(_seed);

        _cellCountX = _chunkCountX * HexMetrics.ChunkSizeX;
        _cellCountZ = _chunckCountZ * HexMetrics.ChunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    private void OnEnable()
    {
        if (!HexMetrics.NoiseSource)
        {
            HexMetrics.NoiseSource = _noiseSource;
            HexMetrics.InitializeHashGrid(_seed);
        }
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
        _chunks = new HexGridChunk[_chunkCountX * _chunckCountZ];

        for (int z = 0, i = 0; z < _chunckCountZ; z++)
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
        cell.Color = _defaultColor;
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
        label.text = cell.Coordinates.ToStringOnSeparateLines();

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
}
