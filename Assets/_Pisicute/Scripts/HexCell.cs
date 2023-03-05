using System.Linq;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates Coordinates;
    public RectTransform UiRect;
    public HexGridChunk Chunk;

    [SerializeField] private HexCell[] _neighbors = new HexCell[6];
    [SerializeField] private bool[] _roads;
    private Color _color;
    private int _elevation = int.MinValue;
    private int _waterLevel;
    private int _urbanLevel;
    private int _farmLevel;
    private int _plantLevel;
    private bool _walled;
    private int _specialIndex;

    public Vector3 Position => transform.localPosition;
    public int Elevation
    {
        get => _elevation;
        set
        {
            if (_elevation == value) return;
            _elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.ElevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ElevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = UiRect.localPosition;
            uiPosition.z = -position.y;
            UiRect.localPosition = uiPosition;

            ValidateRivers();

            for (int i = 0; i < _roads.Length; i++)
            {
                if (_roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
        }
    }
    public Color Color
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value;
            Refresh();
        }
    }
    public bool HasIncomingRiver { get; private set; }
    public bool HasOutgoingRiver { get; private set; }
    public HexDirection IncomingRiver { get; private set; }
    public HexDirection OutgoingRiver { get; private set; }
    public bool HasRiver => HasIncomingRiver || HasOutgoingRiver;
    public bool HasRiverBeginOrEnd => HasIncomingRiver != HasOutgoingRiver;
    public float StreamBedY => (Elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;
    public float RiverSurfaceY => (Elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public bool HasRoads => _roads.Any(t => t);
    public HexDirection RiverBeginOrEndDirection => HasIncomingRiver ? IncomingRiver : OutgoingRiver;
    public int WaterLevel
    {
        get => _waterLevel;
        set
        {
            if (_waterLevel == value) return;
            _waterLevel = value;
            ValidateRivers();
            Refresh();
        }
    }
    public bool IsUnderwater => WaterLevel > Elevation;
    public float WaterSurfaceY => (WaterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public int UrbanLevel
    {
        get => _urbanLevel;
        set
        {
            if (_urbanLevel == value) return;
            _urbanLevel = value;
            RefreshSelfOnly();
        }
    }
    public int FarmLevel
    {
        get => _farmLevel;
        set
        {
            if (_farmLevel == value) return;
            _farmLevel = value;
            RefreshSelfOnly();
        }
    }
    public int PlantLevel
    {
        get => _plantLevel;
        set
        {
            if (_plantLevel == value) return;
            _plantLevel = value;
            RefreshSelfOnly();
        }
    }
    public bool Walled
    {
        get => _walled;
        set
        {
            if (_walled == value) return;
            _walled = value;
            Refresh();
        }
    }
    public int SpecialIndex
    {
        get => _specialIndex;
        set
        {
            if (_specialIndex == value || HasRiver) return;
            _specialIndex = value;
            RemoveRoads();
            RefreshSelfOnly();
        }
    }
    public bool IsSpecial => SpecialIndex > 0;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return _neighbors[(int)direction];
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return HasIncomingRiver && IncomingRiver == direction || HasOutgoingRiver && OutgoingRiver == direction;
    }

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return _roads[(int)direction];
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = Elevation - GetNeighbor(direction).Elevation;
        return difference >= 0 ? difference : -difference;
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        _neighbors[(int)direction] = cell;
        cell._neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(Elevation, _neighbors[(int)direction].Elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
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
        SpecialIndex = 0;

        neighbor.RemoveIncomingRiver();
        neighbor.HasIncomingRiver = true;
        neighbor.IncomingRiver = direction.Opposite();
        neighbor.SpecialIndex = 0;

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

    public void AddRoad(HexDirection direction)
    {
        if (!_roads[(int)direction] && !HasRiverThroughEdge(direction) && !IsSpecial && !GetNeighbor(direction).IsSpecial && GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < _neighbors.Length; i++)
        {
            if (_roads[i])
            {
                SetRoad(i, false);
            }
        }
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

    private void SetRoad(int index, bool state)
    {
        _roads[index] = state;
        _neighbors[index]._roads[(int)((HexDirection)index).Opposite()] = state;
        _neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    private void Refresh()
    {
        if (!Chunk) return;
        Chunk.Refresh();
        foreach (HexCell neighbor in _neighbors)
        {
            if (neighbor != null && neighbor.Chunk != Chunk)
            {
                neighbor.Chunk.Refresh();
            }
        }
    }

    private void RefreshSelfOnly()
    {
        Chunk.Refresh();
    }
}
