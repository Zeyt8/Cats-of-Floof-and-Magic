using System.Linq;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;

public class HexCell : MonoBehaviour, ISaveableObject
{
    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;
    [NonSerialized] public int index;
    [NonSerialized] public HexCellShaderData shaderData;
    [NonSerialized] public int distance;
    [NonSerialized] public HexCell pathFrom;
    [NonSerialized] public int searchHeuristic;
    [NonSerialized] public HexCell nextWithSamePriority;
    [NonSerialized] public int searchPhase;
    [NonSerialized] public UnitObject unit;
    [NonSerialized] public bool isExplorable;

    [SerializeField] private HexCell[] neighbors = new HexCell[6];
    [SerializeField] private bool[] roads = new bool[6];
    [SerializeField] private bool[] walls = new bool[6];
    private int terrainTypeIndex;
    private int elevation = int.MinValue;
    private int waterLevel;
    private int urbanLevel;
    private int farmLevel;
    private int plantLevel;
    private BuildingTypes building;
    private int visibility;
    private bool explored;

    public Vector3 Position => transform.localPosition;
    public int Elevation
    {
        get => elevation;
        set
        {
            if (elevation == value) return;
            elevation = value;
            RefreshPosition();
            ValidateRivers();

            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
        }
    }
    public int TerrainTypeIndex
    {
        get => terrainTypeIndex;
        set
        {
            if (terrainTypeIndex == value) return;
            terrainTypeIndex = value;
            shaderData.RefreshTerrain(this);
        }
    }

    #region River
    public bool HasIncomingRiver { get; private set; }
    public bool HasOutgoingRiver { get; private set; }
    public HexDirection IncomingRiver { get; private set; }
    public HexDirection OutgoingRiver { get; private set; }
    public bool HasRiver => HasIncomingRiver || HasOutgoingRiver;
    public bool HasRiverBeginOrEnd => HasIncomingRiver != HasOutgoingRiver;
    public float StreamBedY => (Elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;
    public float RiverSurfaceY => (Elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public HexDirection RiverBeginOrEndDirection => HasIncomingRiver ? IncomingRiver : OutgoingRiver;

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return HasIncomingRiver && IncomingRiver == direction || HasOutgoingRiver && OutgoingRiver == direction;
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (HasOutgoingRiver && OutgoingRiver == direction) return;
        HexCell neighbor = GetNeighbor(direction);
        if (!IsValidRiverDestination(neighbor)) return;
        RemoveOutgoingRiver();
        if (HasIncomingRiver && IncomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        HasOutgoingRiver = true;
        OutgoingRiver = direction;
        Building = 0;

        neighbor.RemoveIncomingRiver();
        neighbor.HasIncomingRiver = true;
        neighbor.IncomingRiver = direction.Opposite();
        neighbor.Building = 0;

        SetRoad((int)direction, false);
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void RemoveOutgoingRiver()
    {
        if (!HasOutgoingRiver) return;
        HasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(OutgoingRiver);
        neighbor.HasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!HasIncomingRiver) return;
        HasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(IncomingRiver);
        neighbor.HasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    private bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (Elevation >= neighbor.Elevation || WaterLevel == neighbor.Elevation);
    }

    private void ValidateRivers()
    {
        if (HasOutgoingRiver && !IsValidRiverDestination(GetNeighbor(OutgoingRiver)))
        {
            RemoveOutgoingRiver();
        }
        if (HasIncomingRiver && !GetNeighbor(IncomingRiver).IsValidRiverDestination(this))
        {
            RemoveIncomingRiver();
        }
    }
    #endregion

    #region Roads
    public bool HasRoads => roads.Any(t => t);

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction)
    {
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && !IsSpecial && !GetNeighbor(direction).IsSpecial && GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
            }
        }
    }

    private void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
        Refresh();
    }
    #endregion

    public int WaterLevel
    {
        get => waterLevel;
        set
        {
            if (waterLevel == value) return;
            waterLevel = value;
            ValidateRivers();
            Refresh();
        }
    }
    public bool IsUnderwater => WaterLevel > Elevation;
    public float WaterSurfaceY => (WaterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public int UrbanLevel
    {
        get => urbanLevel;
        set
        {
            if (urbanLevel == value) return;
            urbanLevel = value;
            RefreshSelfOnly();
        }
    }
    public int FarmLevel
    {
        get => farmLevel;
        set
        {
            if (farmLevel == value) return;
            farmLevel = value;
            RefreshSelfOnly();
        }
    }
    public int PlantLevel
    {
        get => plantLevel;
        set
        {
            if (plantLevel == value) return;
            plantLevel = value;
            RefreshSelfOnly();
        }
    }
    public BuildingTypes Building
    {
        get => building;
        set
        {
            if (building == value || HasRiver) return;
            building = value;
            RemoveRoads();
            RefreshSelfOnly();
        }
    }
    public bool IsSpecial => Building > 0;
    public int SearchPriority => distance + searchHeuristic;
    public bool IsVisible => visibility > 0 && isExplorable;
    public bool IsExplored { get => explored && isExplorable; private set => explored = value; }
    public int ViewElevation => Elevation >= WaterLevel ? Elevation : WaterLevel;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public HexDirection GetNeighborDirection(HexCell cell)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == cell)
            {
                return (HexDirection)i;
            }
        }

        return HexDirection.NE;
    }

    public bool HasWallThroughEdge(HexDirection direction)
    {
        return walls[(int)direction];
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = Elevation - GetNeighbor(direction).Elevation;
        return difference >= 0 ? difference : -difference;
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(Elevation, neighbors[(int)direction].Elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
    }

    public void AddWall(HexDirection direction)
    {
        if (!HasWallThroughEdge(direction) && GetEdgeType(direction) != HexEdgeType.Cliff && !GetNeighbor(direction).IsUnderwater)
        {
            SetWall((int)direction, true);
        }
    }

    public void RemoveWall()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (walls[i])
            {
                SetWall(i, false);
            }
        }
    }

    private void SetWall(int index, bool state)
    {
        walls[index] = state;
        neighbors[index].walls[(int)((HexDirection)index).Opposite()] = state;
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    private void Refresh()
    {
        if (!chunk) return;
        chunk.Refresh();
        foreach (HexCell neighbor in neighbors)
        {
            if (neighbor != null && neighbor.chunk != chunk)
            {
                neighbor.chunk.Refresh();
            }
        }

        if (unit)
        {
            unit.ValidateLocation();
        }
    }

    private void RefreshSelfOnly()
    {
        chunk.Refresh();
        if (unit)
        {
            unit.ValidateLocation();
        }
    }

    private void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.ElevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ElevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    public void IncreaseVisibility()
    {
        visibility++;
        if (visibility == 1)
        {
            IsExplored = true;
            shaderData.RefreshVisibility(this);
        }
    }

    public void DecreaseVisibility()
    {
        visibility--;
        if (visibility == 0)
        {
            shaderData.RefreshVisibility(this);
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)(elevation + 127));
        writer.Write((byte)waterLevel);
        writer.Write((byte)urbanLevel);
        writer.Write((byte)farmLevel);
        writer.Write((byte)plantLevel);
        writer.Write((byte)building);

        if (HasIncomingRiver)
        {
            writer.Write((byte)(IncomingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        if (HasOutgoingRiver)
        {
            writer.Write((byte)(OutgoingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        int roadFlags = 0;
        for (int i = 0; i < roads.Length; i++)
        {
            if (roads[i])
            {
                roadFlags |= 1 << i;
            }
        }
        writer.Write((byte)roadFlags);
        int wallFlags = 0;
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i])
            {
                wallFlags |= 1 << i;
            }
        }
        writer.Write((byte)wallFlags);
        writer.Write(IsExplored);
    }

    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        terrainTypeIndex = reader.ReadByte();
        shaderData.RefreshTerrain(this);
        elevation = reader.ReadByte() - 127;
        RefreshPosition();
        waterLevel = reader.ReadByte();
        urbanLevel = reader.ReadByte();
        farmLevel = reader.ReadByte();
        plantLevel = reader.ReadByte();
        building = (BuildingTypes)reader.ReadByte();

        byte riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            HasIncomingRiver = true;
            IncomingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            HasIncomingRiver = false;
        }

        riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            HasOutgoingRiver = true;
            OutgoingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            HasOutgoingRiver = false;
        }

        int roadFlags = reader.ReadByte();
        for (int i = 0; i < roads.Length; i++)
        {
            roads[i] = (roadFlags & (1 << i)) != 0;
        }
        int wallFlags = reader.ReadByte();
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = (wallFlags & (1 << i)) != 0;
        }

        IsExplored = reader.ReadBoolean();
        shaderData.RefreshVisibility(this);
    }

    public void DisableHighlight()
    {
        GameObject highlight = uiRect.GetChild(0).gameObject;
        highlight.SetActive(false);
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.gameObject.SetActive(true);
    }

    public void SetLabel(string text)
    {
        TextMeshProUGUI label = uiRect.GetComponent<TextMeshProUGUI>();
        label.text = text;
    }
}
