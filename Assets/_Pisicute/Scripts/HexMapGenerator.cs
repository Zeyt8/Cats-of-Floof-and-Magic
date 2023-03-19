using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    [SerializeField] private int _seed;
    [SerializeField] private bool _useFixedSeed;
    [Range(0, 0.5f), SerializeField] private float _jitterProbability = 0.25f;
    [Range(20, 200), SerializeField] private int _chunkSizeMin = 30;
    [Range(20, 200), SerializeField] private int _chunkSizeMax = 100;
    [Range(0f, 1f), SerializeField] private float _highRiseProbability = 0.25f;
    [Range(0, 0.4f), SerializeField] private float _sinkProbability = 0.2f;
    [Range(0, 100), SerializeField] private int _landPercentage = 50;
    [Range(1, 5), SerializeField] private int _waterLevel = 3;
    [Range(-5, 0), SerializeField] private int _elevationMinimum = -2;
    [Range(0, 15), SerializeField] private int _elevationMaximum = 8;
    [SerializeField] private HexGrid _grid;
    private int _cellCount;
    private HexCellPriorityQueue _searchFrontier = new HexCellPriorityQueue();
    private int _searchFrontierPhase;

    public void GenerateMap(int x, int z)
    {
        Random.State originalRandomState = Random.state;
        if (!_useFixedSeed)
        {
            _seed = Random.Range(0, int.MaxValue);
            _seed ^= (int)System.DateTime.Now.Ticks;
            _seed ^= (int)Time.unscaledTime;
            _seed &= int.MaxValue;
        }
        Random.InitState(_seed);
        _cellCount = x * z;
        _grid.CreateMap(x, z);
        _searchFrontier.Clear();
        for (int i = 0; i < _cellCount; i++)
        {
            _grid.GetCell(i).WaterLevel = _waterLevel;
        }

        CreateLand();
        SetTerrainType();
        for (int i = 0; i < _cellCount; i++)
        {
            _grid.GetCell(i).SearchPhase = 0;
        }

        Random.state = originalRandomState;
    }

    private void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(_cellCount * _landPercentage * 0.01f);
        while (landBudget > 0)
        {
            int chunkSize = Random.Range(_chunkSizeMin, _chunkSizeMax + 1);
            if (Random.value < _sinkProbability)
            {
                landBudget = SinkTerrain(chunkSize, landBudget);
            }
            else
            {
                landBudget = RaiseTerrain(chunkSize, landBudget);
            }
        }
    }

    private int RaiseTerrain(int chunkSize, int budget)
    {
        _searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell();
        firstCell.SearchPhase = _searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        _searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.Coordinates;

        int rise = Random.value < _highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && _searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = originalElevation + rise;
            if (newElevation > _elevationMaximum)
            {
                continue;
            }
            current.Elevation = newElevation;
            if (originalElevation < _waterLevel && newElevation >= _waterLevel && --budget == 0) break;
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.SearchPhase < _searchFrontierPhase)
                {
                    neighbor.SearchPhase = _searchFrontierPhase;
                    neighbor.Distance = neighbor.Coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < _jitterProbability ? 1 : 0;
                    _searchFrontier.Enqueue(neighbor);
                }
            }
        }

        _searchFrontier.Clear();
        return budget;
    }

    private int SinkTerrain(int chunkSize, int budget)
    {
        _searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell();
        firstCell.SearchPhase = _searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        _searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.Coordinates;

        int sink = Random.value < _highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && _searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = current.Elevation - sink;
            if (newElevation < _elevationMinimum)
            {
                continue;
            }
            current.Elevation = newElevation;
            if (originalElevation >= _waterLevel && newElevation < _waterLevel)
            {
                budget++;
            }
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.SearchPhase < _searchFrontierPhase)
                {
                    neighbor.SearchPhase = _searchFrontierPhase;
                    neighbor.Distance = neighbor.Coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < _jitterProbability ? 1 : 0;
                    _searchFrontier.Enqueue(neighbor);
                }
            }
        }

        _searchFrontier.Clear();
        return budget;
    }

    private HexCell GetRandomCell()
    {
        return _grid.GetCell(Random.Range(0, _cellCount));
    }

    private void SetTerrainType()
    {
        for (int i = 0; i < _cellCount; i++)
        {
            HexCell cell = _grid.GetCell(i);
            if (!cell.IsUnderwater)
            {
                cell.TerrainTypeIndex = cell.Elevation - cell.WaterLevel;
            }
        }
    }
}
