using System.Collections.Generic;
using UnityEngine;

public struct Biome
{
    public int terrain;
    public int plant;

    public Biome(int terrain, int plant)
    {
        this.terrain = terrain;
        this.plant = plant;
    }
}

public enum HemisphereMode
{
    Both, North, South
}

public struct MapRegion
{
    public int xMin, xMax, zMin, zMax;
}

[System.Serializable]
public struct ClimateData
{
    public float clouds;
    public float moisture;
}

public class HexMapGenerator : MonoBehaviour
{
    private static float[] TemperatureBands = { 0.1f, 0.3f, 0.6f };
    private static float[] MoistureBands = { 0.12f, 0.28f, 0.85f };
    private static Biome[] Biomes = {
        new Biome(0, 0), new Biome(4, 0), new Biome(4, 0), new Biome(4, 0),
        new Biome(0, 0), new Biome(2, 0), new Biome(2, 1), new Biome(2, 2),
        new Biome(0, 0), new Biome(1, 0), new Biome(1, 1), new Biome(1, 2),
        new Biome(0, 0), new Biome(1, 1), new Biome(1, 2), new Biome(1, 3)
    };

    [SerializeField] private MapGeneratorSettings settings;
    [SerializeField] private HexGrid grid;
    
    
    private int cellCount;
    private int landCells;
    private HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
    private int searchFrontierPhase;
    private int temperatureJitterChannel;
    private List<MapRegion> regions = new List<MapRegion>();

    private List<ClimateData> climate = new List<ClimateData>();
    private List<ClimateData> nextClimate = new List<ClimateData>();

    public void GenerateMap(int x, int z)
    {
        Random.State originalRandomState = Random.state;
        if (!settings.useFixedSeed)
        {
            settings.seed = Random.Range(0, int.MaxValue);
            settings.seed ^= (int)System.DateTime.Now.Ticks;
            settings.seed ^= (int)Time.unscaledTime;
            settings.seed &= int.MaxValue;
        }
        Random.InitState(settings.seed);
        cellCount = x * z;
        grid.CreateMap(x, z);
        searchFrontier.Clear();
        for (int i = 0; i < cellCount; i++)
        {
            grid.GetCell(i).WaterLevel = settings.waterLevel;
        }
        CreateRegions();
        CreateLand();
        ErodeLand();
        CreateClimate();
        CreateRivers();
        SetTerrainType();
        for (int i = 0; i < cellCount; i++)
        {
            grid.GetCell(i).searchPhase = 0;
        }

        Random.state = originalRandomState;
    }

    private void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(cellCount * settings.landPercentage * 0.01f);
        landCells = landBudget;
        for (int guard = 0; guard < 10000; guard++)
        {
            bool sink = Random.value < settings.sinkProbability;
            for (int i = 0; i < regions.Count; i++)
            {
                MapRegion region = regions[i];
                int chunkSize = Random.Range(settings.chunkSizeMin, settings.chunkSizeMax + 1);
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
            landCells -= landBudget;
        }
    }

    private int RaiseTerrain(int chunkSize, int budget, MapRegion region)
    {
        searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell(region);
        firstCell.searchPhase = searchFrontierPhase;
        firstCell.distance = 0;
        firstCell.searchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.coordinates;

        int rise = Random.value < settings.highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = originalElevation + rise;
            if (newElevation > settings.elevationMaximum)
            {
                continue;
            }
            current.Elevation = newElevation;
            if (originalElevation < settings.waterLevel && newElevation >= settings.waterLevel && --budget == 0) break;
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.searchPhase < searchFrontierPhase)
                {
                    neighbor.searchPhase = searchFrontierPhase;
                    neighbor.distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.searchHeuristic = Random.value < settings.jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }

        searchFrontier.Clear();
        return budget;
    }

    private int SinkTerrain(int chunkSize, int budget, MapRegion region)
    {
        searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell(region);
        firstCell.searchPhase = searchFrontierPhase;
        firstCell.distance = 0;
        firstCell.searchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.coordinates;

        int sink = Random.value < settings.highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = current.Elevation - sink;
            if (newElevation < settings.elevationMinimum)
            {
                continue;
            }
            current.Elevation = newElevation;
            if (originalElevation >= settings.waterLevel && newElevation < settings.waterLevel)
            {
                budget++;
            }
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.searchPhase < searchFrontierPhase)
                {
                    neighbor.searchPhase = searchFrontierPhase;
                    neighbor.distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.searchHeuristic = Random.value < settings.jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }

        searchFrontier.Clear();
        return budget;
    }

    private void ErodeLand()
    {
        List<HexCell> erodibleCells = ListPool<HexCell>.Get();
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            if (IsErodible(cell))
            {
                erodibleCells.Add(cell);
            }
        }
        int targetErodibleCount = (int)(erodibleCells.Count * (100 - settings.erosionPercentage) * 0.01f);

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
        return grid.GetCell(Random.Range(region.xMin, region.xMax), Random.Range(region.zMin, region.zMax));
    }

    private void SetTerrainType()
    {
        temperatureJitterChannel = Random.Range(0, 4);
        //int rockDesertElevation = settings.elevationMaximum - (settings.elevationMaximum - settings.waterLevel) / 2;
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            float temperature = DetermineTemperature(cell);
            float moisture = climate[i].moisture;
            if (!cell.IsUnderwater)
            {
                int t = 0;
                for (; t < TemperatureBands.Length; t++)
                {
                    if (temperature < TemperatureBands[t]) break;
                }

                int m = 0;
                for (; m < MoistureBands.Length; m++)
                {
                    if (moisture < MoistureBands[m]) break;
                }
                
                Biome cellBiome = Biomes[t * 4 + m];
                if (cellBiome.terrain == 0)
                {
                    if (cell.Elevation >= settings.rockDesertElevation)
                    {
                        cellBiome.terrain = 3;
                    }
                }
                else if (cell.Elevation == settings.elevationMaximum)
                {
                    cellBiome.terrain = 4;
                }
                if (cellBiome.terrain == 4)
                {
                    cellBiome.plant = 0;
                }
                else if (cellBiome.plant < 3 && cell.HasRiver)
                {
                    cellBiome.plant += 1;
                }
                cell.TerrainTypeIndex = cellBiome.terrain;
                cell.PlantLevel = cellBiome.plant;
            }
            else
            {
                int terrain;
                if (cell.Elevation == settings.waterLevel - 1)
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
                else if (cell.Elevation >= settings.waterLevel)
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
                if (terrain == 1 && temperature < TemperatureBands[0])
                {
                    terrain = 2;
                }
                cell.TerrainTypeIndex = terrain;
            }
        }
    }

    private void CreateRegions()
    {
        regions.Clear();

        MapRegion region;
        switch (settings.regionCount)
        {
            default:
                region.xMin = settings.mapBorderX;
                region.xMax = grid.cellCountX - settings.mapBorderX;
                region.zMin = settings.mapBorderZ;
                region.zMax = grid.cellCountZ - settings.mapBorderZ;
                regions.Add(region);
                break;
            case 2:
                if (Random.value < 0.5f)
                {
                    region.xMin = settings.mapBorderX;
                    region.xMax = grid.cellCountX / 2 - settings.regionBorder;
                    region.zMin = settings.mapBorderZ;
                    region.zMax = grid.cellCountZ - settings.mapBorderZ;
                    regions.Add(region);
                    region.xMin = grid.cellCountX / 2 + settings.regionBorder;
                    region.xMax = grid.cellCountX - settings.mapBorderX;
                    regions.Add(region);
                }
                else
                {
                    region.xMin = settings.mapBorderX;
                    region.xMax = grid.cellCountX - settings.mapBorderX;
                    region.zMin = settings.mapBorderZ;
                    region.zMax = grid.cellCountZ / 2 - settings.regionBorder;
                    regions.Add(region);
                    region.zMin = grid.cellCountZ / 2 + settings.regionBorder;
                    region.zMax = grid.cellCountZ - settings.mapBorderZ;
                    regions.Add(region);
                }
                break;
            case 3:
                region.xMin = settings.mapBorderX;
                region.xMax = grid.cellCountX / 3 - settings.regionBorder;
                region.zMin = settings.mapBorderZ;
                region.zMax = grid.cellCountZ - settings.mapBorderZ;
                regions.Add(region);
                region.xMin = grid.cellCountX / 3 + settings.regionBorder;
                region.xMax = grid.cellCountX * 2 / 3 - settings.regionBorder;
                regions.Add(region);
                region.xMin = grid.cellCountX * 2 / 3 + settings.regionBorder;
                region.xMax = grid.cellCountX - settings.mapBorderX;
                regions.Add(region);
                break;
            case 4:
                region.xMin = settings.mapBorderX;
                region.xMax = grid.cellCountX / 2 - settings.regionBorder;
                region.zMin = settings.mapBorderZ;
                region.zMax = grid.cellCountZ / 2 - settings.regionBorder;
                regions.Add(region);
                region.xMin = grid.cellCountX / 2 + settings.regionBorder;
                region.xMax = grid.cellCountX - settings.mapBorderX;
                regions.Add(region);
                region.zMin = grid.cellCountZ / 2 + settings.regionBorder;
                region.zMax = grid.cellCountZ - settings.mapBorderZ;
                regions.Add(region);
                region.xMin = settings.mapBorderX;
                region.xMax = grid.cellCountX / 2 - settings.regionBorder;
                regions.Add(region);
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
        climate.Clear();
        nextClimate.Clear();
        ClimateData initialData = new ClimateData();
        initialData.moisture = settings.startingMoisture;
        ClimateData clearData = new ClimateData();
        for (int i = 0; i < cellCount; i++)
        {
            climate.Add(initialData);
            nextClimate.Add(clearData);
        }

        for (int cycle = 0; cycle < 40; cycle++)
        {
            for (int i = 0; i < cellCount; i++)
            {
                EvolveClimate(i);
            }

            (climate, nextClimate) = (nextClimate, climate);
        }
    }

    private void EvolveClimate(int cellIndex)
    {
        HexCell cell = grid.GetCell(cellIndex);
        ClimateData cellClimate = climate[cellIndex];

        if (cell.IsUnderwater)
        {
            cellClimate.moisture = 1;
            cellClimate.clouds += settings.evaporationFactor;
        }
        else
        {
            float evaporation = cellClimate.moisture * settings.evaporationFactor;
            cellClimate.moisture -= evaporation;
            cellClimate.clouds += evaporation;
        }

        float precipitation = cellClimate.clouds * settings.precipitationFactor;
        cellClimate.clouds -= precipitation;
        cellClimate.moisture += precipitation;

        float cloudMaximum = 1f - cell.ViewElevation / (settings.elevationMaximum + 1f);
        if (cellClimate.clouds > cloudMaximum)
        {
            cellClimate.moisture += cellClimate.clouds - cloudMaximum;
            cellClimate.clouds = cloudMaximum;
        }

        HexDirection mainDispersalDirection = settings.windDirection.Opposite();
        float cloudDispersal = cellClimate.clouds / (5 + settings.windStrength);
        float runoff = cellClimate.moisture * settings.runoffFactor / 6;
        float seepage = cellClimate.moisture * settings.seepageFactor / 6;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (!neighbor)
            {
                continue;
            }

            ClimateData neighborClimate = nextClimate[neighbor.index];
            if (d == mainDispersalDirection)
            {
                neighborClimate.clouds += cloudDispersal * settings.windStrength;
            }
            else
            {
                neighborClimate.clouds += cloudDispersal;
            }

            int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
            if (elevationDelta < 0)
            {
                cellClimate.moisture -= runoff;
                neighborClimate.moisture += runoff;
            }
            else if (elevationDelta == 0)
            {
                cellClimate.moisture -= seepage;
                neighborClimate.moisture += seepage;
            }

            nextClimate[neighbor.index] = neighborClimate;
        }

        ClimateData nextCellClimate = nextClimate[cellIndex];
        nextCellClimate.moisture += cellClimate.moisture;
        if (nextCellClimate.moisture > 1f)
        {
            nextCellClimate.moisture = 1f;
        }
        nextClimate[cellIndex] = nextCellClimate;
        climate[cellIndex] = new ClimateData();
    }

    private void CreateRivers()
    {
        List<HexCell> riverOrigins = ListPool<HexCell>.Get();
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            if (cell.IsUnderwater)
            {
                continue;
            }

            ClimateData data = climate[i];
            float weight = data.moisture * (cell.Elevation - settings.waterLevel) / (settings.elevationMaximum - settings.waterLevel);
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

        int riverBudget = Mathf.RoundToInt(landCells * settings.riverPercentage * 0.01f);
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

            if (minNeighborElevation >= cell.Elevation && Random.value < settings.extraLakeProbability)
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
        float latitude = (float)cell.coordinates.Z / grid.cellCountZ;
        if (settings.hemisphere == HemisphereMode.Both)
        {
            latitude *= 2f;
            if (latitude > 1f)
            {
                latitude = 2f - latitude;
            }
        }
        else if (settings.hemisphere == HemisphereMode.North)
        {
            latitude = 1f - latitude;
        }
        float temperature = Mathf.LerpUnclamped(settings.lowTemperature, settings.highTemperature, latitude);
        temperature *= 1f - (cell.ViewElevation - settings.waterLevel) / (settings.elevationMaximum - settings.waterLevel + 1f);
        float jitter = HexMetrics.SampleNoise(cell.Position * 0.1f)[temperatureJitterChannel];
        temperature += (jitter * 2f - 1f) * settings.temperatureJitter;
        return temperature;
    }
}
