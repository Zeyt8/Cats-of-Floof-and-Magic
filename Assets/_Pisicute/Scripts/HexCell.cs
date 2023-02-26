using System.Linq;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates Coordinates;
    public RectTransform UiRect;
    public HexGridChunk Chunk;
    public Color _color;

    [SerializeField] private HexCell[] _neighbors = new HexCell[6];
    [SerializeField] private bool[] _roads;
    private int _elevation = int.MinValue;
    private bool _hasIncomingRiver;
    private bool _hasOutgoingRiver;
    private HexDirection _incomingRiver;
    private HexDirection _outgoingRiver;

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

            if (_hasOutgoingRiver && _elevation < GetNeighbor(_outgoingRiver).Elevation)
            {
                RemoveOutgoingRiver();
            }
            if (_hasIncomingRiver && _elevation > GetNeighbor(_incomingRiver).Elevation)
            {
                RemoveIncomingRiver();
            }

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
    public bool HasIncomingRiver => _hasIncomingRiver;

    public bool HasOutgoingRiver => _hasOutgoingRiver;

    public HexDirection IncomingRiver => _incomingRiver;

    public HexDirection OutgoingRiver => _outgoingRiver;

    public bool HasRiver => _hasIncomingRiver || _hasOutgoingRiver;
    public bool HasRiverBeginOrEnd => _hasIncomingRiver != _hasOutgoingRiver;
    public float StreamBedY => (_elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;
    public float RiverSurfaceY => (_elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public bool HasRoads => _roads.Any(t => t);
    public HexDirection RiverBeginOrEndDirection => _hasIncomingRiver ? _incomingRiver : _outgoingRiver;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return _neighbors[(int)direction];
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return _hasIncomingRiver && _incomingRiver == direction || _hasOutgoingRiver && _outgoingRiver == direction;
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
        if (_hasOutgoingRiver && _outgoingRiver == direction) return;
        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || Elevation < neighbor.Elevation) return;
        RemoveOutgoingRiver();
        if (_hasIncomingRiver && _incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        _hasOutgoingRiver = true;
        _outgoingRiver = direction;

        neighbor.RemoveIncomingRiver();
        neighbor._hasIncomingRiver = true;
        neighbor._incomingRiver = direction.Opposite();

        SetRoad((int)direction, false);
    }

    public void RemoveRiver()
    {

        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void RemoveOutgoingRiver()
    {
        if (!_hasOutgoingRiver) return;
        _hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(_outgoingRiver);
        neighbor._hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!_hasIncomingRiver) return;
        _hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(_incomingRiver);
        neighbor._hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void AddRoad(HexDirection direction)
    {
        if (!_roads[(int)direction] && !HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1)
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
