using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnitObject : MonoBehaviour, ISaveableObject
{
    public int Speed = 24;
    [NonSerialized] public HexGrid Grid;
    private const float TravelSpeed = 4f;
    private const float RotationSpeed = 180f;

    private HexCell _location;
    private float _orientation;
    private List<HexCell> _pathToTravel;
    public int VisionRange => 3;
    public HexCell Location
    {
        get => _location;
        set
        {
            if (_location)
            {
                Grid.DecreaseVisibility(_location, VisionRange);
                _location.Unit = null;
            }
            _location = value;
            value.Unit = this;
            Grid.IncreaseVisibility(value, VisionRange);
            transform.localPosition = value.Position;
        }
    }
    public float Orientation
    {
        get => _orientation;
        set
        {
            _orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public void Die()
    {
        if (Location)
        {
            Grid.DecreaseVisibility(Location, VisionRange);
        }
        Location.Unit = null;
        Destroy(gameObject);
    }

    public void ValidateLocation()
    {
        transform.localPosition = Location.Position;
    }

    public bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored && !cell.IsUnderwater && !cell.Unit;
    }

    public bool IsValidCrossing(HexCell fromCell, HexCell toCell)
    {
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        return edgeType != HexEdgeType.Cliff && !fromCell.HasWallThroughEdge(fromCell.GetNeighborDirection(toCell));
    }

    public int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        
        if (!IsValidCrossing(fromCell, toCell)) return -1;
        
        int moveCost;
        if (fromCell.HasRoadThroughEdge(direction))
        {
            moveCost = 1;
        }
        else
        {
            moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            moveCost += toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
        }

        return moveCost;
    }

    public void Travel(List<HexCell> path)
    {
        _location.Unit = null;
        _location = path[^1];
        _location.Unit = this;
        _pathToTravel = path;
        StartCoroutine(TravelPath());
    }

    private IEnumerator TravelPath()
    {
        Vector3 a, b, c = _pathToTravel[0].Position;
        yield return LookAt(_pathToTravel[1].Position);
        Grid.DecreaseVisibility(_pathToTravel[0], VisionRange);

        float t = Time.deltaTime * TravelSpeed;
        for (int i = 1; i < _pathToTravel.Count; i++)
        {
            a = c;
            b = _pathToTravel[i - 1].Position;
            c = (b + _pathToTravel[i].Position) * 0.5f;
            Grid.IncreaseVisibility(_pathToTravel[i], VisionRange);
            for (; t < 1f; t += Time.deltaTime * TravelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisibility(_pathToTravel[i], VisionRange);
            t -= 1f;
        }

        a = c;
        b = _location.Position;
        c = b;
        Grid.IncreaseVisibility(_location, VisionRange);
        for (; t < 1f; t += Time.deltaTime * TravelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = _location.Position;
        Orientation = transform.localRotation.eulerAngles.y;

        ListPool<HexCell>.Add(_pathToTravel);
        _pathToTravel = null;
    }

    private IEnumerator LookAt(Vector3 point)
    {
        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation = Quaternion.LookRotation(point - transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);
        if (angle > 0)
        {
            float speed = RotationSpeed / angle;

            for (float t = Time.deltaTime * speed; t < 1f; t += Time.deltaTime * speed)
            {
                transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }

            transform.LookAt(point);
            _orientation = transform.localRotation.eulerAngles.y;
        }
    }

    public void Save(BinaryWriter writer)
    {
        Location.Coordinates.Save(writer);
        writer.Write(Orientation);
    }
    
    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(this, grid.GetCell(coordinates), orientation);
    }
}
