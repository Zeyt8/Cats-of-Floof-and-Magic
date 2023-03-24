using System.Collections.Generic;
using UnityEngine;

public struct Biome
{
    public int Terrain;
    public int Plant;

    public Biome(int terrain, int plant)
    {
        Terrain = terrain;
        Plant = plant;
    }
}

public enum HemisphereMode
{
    Both, North, South
}

public struct MapRegion
{
    public int XMin, XMax, ZMin, ZMax;
}

public struct ClimateData
{
    public float Clouds;
    public float Moisture;
}

public class HexMapGenerator : MonoBehaviour
{
    private static float[] _temperatureBands = { 0.1f, 0.3f, 0.6f };
    private static float[] _moistureBands = { 0.12f, 0.28f, 0.85f };
    private static Biome[] _biomes = {
        new Biome(0, 0), new Biome(4, 0), new Biome(4, 0), new Biome(4, 0),
        new Biome(0, 0), new Biome(2, 0), new Biome(2, 1), new Biome(2, 2),
        new Biome(0, 0), new Biome(1, 0), new Biome(1, 1), new Biome(1, 2),
        new Biome(0, 0), new Biome(1, 1), new Biome(1, 2), new Biome(1, 3)
    };

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
    [Range(0, 10), SerializeField] private int _mapBorderX = 5;
    [Range(0, 10), SerializeField] private int _mapBorderZ = 5;
    [Range(0, 10), SerializeField] private int _regionBorder = 5;
    [Range(1, 4), SerializeField] private int _regionCount = 1;
    [Range(0, 100), SerializeField] private int _erosionPercentage = 50;
    [Range(0f, 1f), SerializeField] private float _precipitationFactor = 0.25f;
    [Range(0f, 1f), SerializeField] private float _evaporationFactor = 0.5f;
    [Range(0f, 1f), SerializeField] private float _runoffFactor = 0.25f;
    [Range(0f, 1f), SerializeField] private float _seepageFactor = 0.125f;
    [SerializeField] private HexDirection _windDirection = HexDirection.NW;
    [Range(1f, 10f), SerializeField] private float _windStrength = 4f;
    [Range(0f, 1f), SerializeField] private float _startingMoisture = 0.1f;
    [Range(0, 40), SerializeField] private int _riverPercentage = 10;
    [Range(0f, 1f), SerializeField] private float _extraLakeProbability = 0.25f;
    [Range(0f, 1f), SerializeField] private float _lowTemperature = 0f;
    [Range(0f, 1f), SerializeField] private float _highTemperature = 1f;
    [Range(0f, 1f), SerializeField] private float _temperatureJitter = 0.1f;
    [SerializeField] private HemisphereMode _hemisphere;
    [SerializeField] private HexGrid _grid;
    
    private int _cellCount;
    private int _landCells;
    private HexCellPriorityQueue _searchFrontier = new HexCellPriorityQueue();
    private int _searchFrontierPhase;
    private int _temperatureJitterChannel;
    private List<MapRegion> _regions = new List<MapRegion>();

    private List<ClimateData> _climate = new List<ClimateData>();
    private List<ClimateData> _nextClimate = new List<ClimateData>();

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
        CreateRegions();
        CreateLand();
        ErodeLand();
        CreateClimate();
        CreateRivers();
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
        _landCells = landBudget;
        for (int guard = 0; guard < 10000; guard++)
        {
            bool sink = Random.value < _sinkProbability;
            for (int i = 0; i < _regions.Count; i++)
            {
                MapRegion region = _regions[i];
                int chunkSize = Random.Range(_chunkSizeMin, _chunkSizeMax + 1);
                if (sink)
                {
                    landBudget = SinkTerrain(chunkSize, landBudget, region);
                }
                else
                {
                    landBudget = RaiseTerrain(chunkSize, landBudget, region);
                    if (landBudget == 0)
                    {
                        return;
                    }
                }
            }
        }
        if (landBudget > 0)
        {
            Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
            _landCells -= landBudget;
        }
    }

    private int RaiseTerrain(int chunkSize, int budget, MapRegion region)
    {
        _searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell(region);
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

    private int SinkTerrain(int chunkSize, int budget, MapRegion region)
    {
        _searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell(region);
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

    private void ErodeLand()
    {
        List<HexCell> erodibleCells = ListPool<HexCell>.Get();
        for (int i = 0; i < _cellCount; i++)
        {
            HexCell cell = _grid.GetCell(i);
            if (IsErodible(cell))
            {
                erodibleCells.Add(cell);
            }
        }
        int targetErodibleCount = (int)(erodibleCells.Count * (100 - _erosionPercentage) * 0.01f);

        while (erodibleCells.Count > targetErodibleCount)
        {
            int index = Random.Range(0, erodibleCells.Count);
            HexCell cell = erodibleCells[index];
            HexCell targetCell = GetErosionTarget(cell);
            cell.Elevation -= 1;
            targetCell.Elevation += 1;
            if (!IsErodible(cell))
            {
                erodibleCells[index] = erodibleCells[^1];
                erodibleCells.RemoveAt(erodibleCells.Count - 1);
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation == cell.Elevation + 2 && !erodibleCells.Contains(neighbor))
                {
                    erodibleCells.Add(neighbor);
                }
            }
            if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell))
            {
                erodibleCells.Add(targetCell);
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = targetCell.GetNeighbor(d);
                if (neighbor && neighbor != cell && neighbor.Elevation == targetCell.Elevation + 1 && !IsErodible(neighbor))
                {
                    erodibleCells.Remove(neighbor);
                }
            }
        }

        ListPool<HexCell>.Add(erodibleCells);
    }

    private HexCell GetRandomCell(MapRegion region)
    {
        return _grid.GetCell(Random.Range(region.XMin, region.XMax), Random.Range(region.ZMin, region.ZMax));
    }

    private void SetTerrainType()
    {
        _temperatureJitterChannel = Random.Range(0, 4);
        int rockDesertElevation = _elevationMaximum - (_elevationMaximum - _waterLevel) / 2;
        for (int i = 0; i < _cellCount; i++)
        {
            HexCell cell = _grid.GetCell(i);
            float temperature = DetermineTemperature(cell);
            float moisture = _climate[i].Moisture;
            if (!cell.IsUnderwater)
            {
                int t = 0;
                for (; t < _temperatureBands.Length; t++)
                {
                    if (temperature < _temperatureBands[t]) break;
                }

                int m = 0;
                for (; m < _moistureBands.Length; m++)
                {
                    if (moisture < _moistureBands[m]) break;
                }

                Biome cellBiome = _biomes[t * 4 + m];
                if (cellBiome.Terrain == 0 && cell.Elevation >= rockDesertElevation)
                {
                    cellBiome.Terrain = 3;
                }
                else if (cell.Elevation == _elevationMaximum)
                {
                    cellBiome.Terrain = 4;
                }
                if (cellBiome.Terrain == 4)
                {
                    cellBiome.Plant = 0;
                }
                else if (cellBiome.Plant < 3 && cell.HasRiver)
                {
                    cellBiome.Plant += 1;
                }
                cell.TerrainTypeIndex = cellBiome.Terrain;
                cell.PlantLevel = cellBiome.Plant;
            }
            else
            {
                int terrain;
                if (cell.Elevation == _waterLevel - 1)
                {
                    int cliffs = 0;
                    int slopes = 0;
                    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                    {
                        HexCell neighbor = cell.GetNeighbor(d);
                        if (!neighbor) continue;
                        int delta = neighbor.Elevation - cell.WaterLevel;
                        if (delta == 0)
                        {
                            slopes += 1;
                        }
                        else if (delta > 0)
                        {
                            cliffs += 1;
                        }
                    }
                    if (cliffs + slopes > 3)
                    {
                        terrain = 1;
                    }
                    else if (cliffs > 0)
                    {
                        terrain = 3;
                    }
                    else if (slopes > 0)
                    {
                        terrain = 0;
                    }
                    else
                    {
                        terrain = 1;
                    }
                }
                else if (cell.Elevation >= _waterLevel)
                {
                    terrain = 1;
                }
                else if (cell.Elevation < 0)
                {
                    terrain = 3;
                }
                else
                {
                    terrain = 2;
                }
                if (terrain == 1 && temperature < _temperatureBands[0])
                {
                    terrain = 2;
                }
                cell.TerrainTypeIndex = terrain;
            }
        }
    }

    private void CreateRegions()
    {
        _regions.Clear();

        MapRegion region;
        switch (_regionCount)
        {
            default:
                region.XMin = _mapBorderX;
                region.XMax = _grid.CellCountX - _mapBorderX;
                region.ZMin = _mapBorderZ;
                region.ZMax = _grid.CellCountZ - _mapBorderZ;
                _regions.Add(region);
                break;
            case 2:
                if (Random.value < 0.5f)
                {
                    region.XMin = _mapBorderX;
                    region.XMax = _grid.CellCountX / 2 - _regionBorder;
                    region.ZMin = _mapBorderZ;
                    region.ZMax = _grid.CellCountZ - _mapBorderZ;
                    _regions.Add(region);
                    region.XMin = _grid.CellCountX / 2 + _regionBorder;
                    region.XMax = _grid.CellCountX - _mapBorderX;
                    _regions.Add(region);
                }
                else
                {
                    region.XMin = _mapBorderX;
                    region.XMax = _grid.CellCountX - _mapBorderX;
                    region.ZMin = _mapBorderZ;
                    region.ZMax = _grid.CellCountZ / 2 - _regionBorder;
                    _regions.Add(region);
                    region.ZMin = _grid.CellCountZ / 2 + _regionBorder;
                    region.ZMax = _grid.CellCountZ - _mapBorderZ;
                    _regions.Add(region);
                }
                break;
            case 3:
                region.XMin = _mapBorderX;
                region.XMax = _grid.CellCountX / 3 - _regionBorder;
                region.ZMin = _mapBorderZ;
                region.ZMax = _grid.CellCountZ - _mapBorderZ;
                _regions.Add(region);
                region.XMin = _grid.CellCountX / 3 + _regionBorder;
                region.XMax = _grid.CellCountX * 2 / 3 - _regionBorder;
                _regions.Add(region);
                region.XMin = _grid.CellCountX * 2 / 3 + _regionBorder;
                region.XMax = _grid.CellCountX - _mapBorderX;
                _regions.Add(region);
                break;
            case 4:
                region.XMin = _mapBorderX;
                region.XMax = _grid.CellCountX / 2 - _regionBorder;
                region.ZMin = _mapBorderZ;
                region.ZMax = _grid.CellCountZ / 2 - _regionBorder;
                _regions.Add(region);
                region.XMin = _grid.CellCountX / 2 + _regionBorder;
                region.XMax = _grid.CellCountX - _mapBorderX;
                _regions.Add(region);
                region.ZMin = _grid.CellCountZ / 2 + _regionBorder;
                region.ZMax = _grid.CellCountZ - _mapBorderZ;
                _regions.Add(region);
                region.XMin = _mapBorderX;
                region.XMax = _grid.CellCountX / 2 - _regionBorder;
                _regions.Add(region);
                break;
        }
    }

    private bool IsErodible(HexCell cell)
    {
        int erodibleElevation = cell.Elevation - 2;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor && neighbor.Elevation <= erodibleElevation)
            {
                return true;
            }
        }
        return false;
    }

    private HexCell GetErosionTarget(HexCell cell)
    {
        List<HexCell> candidates = ListPool<HexCell>.Get();
        int erodibleElevation = cell.Elevation - 2;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor && neighbor.Elevation <= erodibleElevation)
            {
                candidates.Add(neighbor);
            }
        }
        HexCell target = candidates[Random.Range(0, candidates.Count)];
        ListPool<HexCell>.Add(candidates);
        return target;
    }

    private void CreateClimate()
    {
        _climate.Clear();
        _nextClimate.Clear();
        ClimateData initialData = new ClimateData();
        initialData.Moisture = _startingMoisture;
        ClimateData clearData = new ClimateData();
        for (int i = 0; i < _cellCount; i++)
        {
            _climate.Add(initialData);
            _nextClimate.Add(clearData);
        }

        for (int cycle = 0; cycle < 40; cycle++)
        {
            for (int i = 0; i < _cellCount; i++)
            {
                EvolveClimate(i);
            }

            (_climate, _nextClimate) = (_nextClimate, _climate);
        }
    }

    private void EvolveClimate(int cellIndex)
    {
        HexCell cell = _grid.GetCell(cellIndex);
        ClimateData cellClimate = _climate[cellIndex];

        if (cell.IsUnderwater)
        {
            cellClimate.Moisture = 1;
            cellClimate.Clouds += _evaporationFactor;
        }
        else
        {
            float evaporation = cellClimate.Moisture * _evaporationFactor;
            cellClimate.Moisture -= evaporation;
            cellClimate.Clouds += evaporation;
        }

        float precipitation = cellClimate.Clouds * _precipitationFactor;
        cellClimate.Clouds -= precipitation;
        cellClimate.Moisture += precipitation;

        float cloudMaximum = 1f - cell.ViewElevation / (_elevationMaximum + 1f);
        if (cellClimate.Clouds > cloudMaximum)
        {
            cellClimate.Moisture += cellClimate.Clouds - cloudMaximum;
            cellClimate.Clouds = cloudMaximum;
        }

        HexDirection mainDispersalDirection = _windDirection.Opposite();
        float cloudDispersal = cellClimate.Clouds / (5 + _windStrength);
        float runoff = cellClimate.Moisture * _runoffFactor / 6;
        float seepage = cellClimate.Moisture * _seepageFactor / 6;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (!neighbor)
            {
                continue;
            }

            ClimateData neighborClimate = _nextClimate[neighbor.Index];
            if (d == mainDispersalDirection)
            {
                neighborClimate.Clouds += cloudDispersal * _windStrength;
            }
            else
            {
                neighborClimate.Clouds += cloudDispersal;
            }

            int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
            if (elevationDelta < 0)
            {
                cellClimate.Moisture -= runoff;
                neighborClimate.Moisture += runoff;
            }
            else if (elevationDelta == 0)
            {
                cellClimate.Moisture -= seepage;
                neighborClimate.Moisture += seepage;
            }

            _nextClimate[neighbor.Index] = neighborClimate;
        }

        ClimateData nextCellClimate = _nextClimate[cellIndex];
        nextCellClimate.Moisture += cellClimate.Moisture;
        if (nextCellClimate.Moisture > 1f)
        {
            nextCellClimate.Moisture = 1f;
        }
        _nextClimate[cellIndex] = nextCellClimate;
        _climate[cellIndex] = new ClimateData();
    }

    private void CreateRivers()
    {
        List<HexCell> riverOrigins = ListPool<HexCell>.Get();
        for (int i = 0; i < _cellCount; i++)
        {
            HexCell cell = _grid.GetCell(i);
            if (cell.IsUnderwater)
            {
                continue;
            }

            ClimateData data = _climate[i];
            float weight = data.Moisture * (cell.Elevation - _waterLevel) / (_elevationMaximum - _waterLevel);
            if (weight > 0.75f)
            {
                riverOrigins.Add(cell);
                riverOrigins.Add(cell);
            }
            if (weight > 0.5f)
            {
                riverOrigins.Add(cell);
            }
            if (weight > 0.25f)
            {
                riverOrigins.Add(cell);
            }
        }

        int riverBudget = Mathf.RoundToInt(_landCells * _riverPercentage * 0.01f);
        while (riverBudget > 0 && riverOrigins.Count > 0)
        {
            int index = Random.Range(0, riverOrigins.Count);
            HexCell origin = riverOrigins[index];
            riverOrigins[index] = riverOrigins[^1];
            riverOrigins.RemoveAt(riverOrigins.Count - 1);

            if (!origin.HasRiver)
            {
                bool isValidOrigin = true;
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = origin.GetNeighbor(d);
                    if (neighbor && (neighbor.HasRiver || neighbor.IsUnderwater))
                    {
                        isValidOrigin = false;
                        break;
                    }
                }
                if (isValidOrigin)
                {
                    riverBudget -= CreateRiver(origin);

                }
            }
        }

        if (riverBudget > 0)
        {
            Debug.LogWarning("Failed to use up river budget.");
        }

        ListPool<HexCell>.Add(riverOrigins);
    }

    private int CreateRiver(HexCell origin)
    {
        int length = 1;
        HexCell cell = origin;
        HexDirection direction = HexDirection.NE;
        List<HexDirection> flowDirections = new List<HexDirection>();

        while (!cell.IsUnderwater)
        {
            int minNeighborElevation = int.MaxValue;
            flowDirections.Clear();
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (!neighbor) continue;
                if (neighbor.Elevation < minNeighborElevation)
                {
                    minNeighborElevation = neighbor.Elevation;
                }
                if (neighbor == origin || neighbor.HasIncomingRiver) continue;
                
                if (neighbor.Elevation - cell.Elevation < 0)
                {
                    flowDirections.Add(d);
                    flowDirections.Add(d);
                    flowDirections.Add(d);
                }

                if (neighbor.HasOutgoingRiver)
                {
                    cell.SetOutgoingRiver(d);
                    return length;
                }

                if (length == 1 || (d != direction.Next2() && d != direction.Previous2()))
                {
                    flowDirections.Add(d);
                }
                flowDirections.Add(d);
            }
            
            if (flowDirections.Count == 0)
            {
                if (length == 1)
                {
                    return 0;
                }

                if (minNeighborElevation >= cell.Elevation)
                {
                    cell.WaterLevel = minNeighborElevation;
                    if (minNeighborElevation == cell.Elevation)
                    {
                        cell.Elevation = minNeighborElevation - 1;
                    }
                }
                break;
            }
            
            direction = flowDirections[Random.Range(0, flowDirections.Count)];
            cell.SetOutgoingRiver(direction);
            length += 1;

            if (minNeighborElevation >= cell.Elevation && Random.value < _extraLakeProbability)
            {
                cell.WaterLevel = cell.Elevation;
                cell.Elevation -= 1;
            }
            
            cell = cell.GetNeighbor(direction);
        }

        return length;
    }

    private float DetermineTemperature(HexCell cell)
    {
        float latitude = (float)cell.Coordinates.Z / _grid.CellCountZ;
        if (_hemisphere == HemisphereMode.Both)
        {
            latitude *= 2f;
            if (latitude > 1f)
            {
                latitude = 2f - latitude;
            }
        }
        else if (_hemisphere == HemisphereMode.North)
        {
            latitude = 1f - latitude;
        }
        float temperature = Mathf.LerpUnclamped(_lowTemperature, _highTemperature, latitude);
        temperature *= 1f - (cell.ViewElevation - _waterLevel) / (_elevationMaximum - _waterLevel + 1f);
        float jitter = HexMetrics.SampleNoise(cell.Position * 0.1f)[_temperatureJitterChannel];
        temperature += (jitter * 2f - 1f) *_temperatureJitter;
        return temperature;
    }
}
