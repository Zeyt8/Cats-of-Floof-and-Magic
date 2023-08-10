using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnitObject : MonoBehaviour, ISaveableObject
{
    public int owner;
    public virtual int Speed => 24;
    public bool IsMoving = false;
    [NonSerialized] public HexGrid grid;
    private const float TravelSpeed = 4f;
    private const float RotationSpeed = 180f;

    private HexCell location;
    private float orientation;
    private List<HexCell> pathToTravel;
    public int visionRange = 3;
    public HexCell Location
    {
        get => location;
        set
        {
            if (location)
            {
                grid.DecreaseVisibility(location, visionRange);
                location.units.Remove(this);
            }
            location = value;
            value.units.Add(this);
            if (Player.Instance && owner == Player.Instance.playerNumber)
            {
                grid.IncreaseVisibility(value, visionRange);
            }
            transform.localPosition = value.Position;
            if (location.Building != null)
            {
                location.Building.OnUnitEnter(this);
            }
        }
    }
    public float Orientation
    {
        get => orientation;
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public virtual void Die()
    {
    }

    public void ValidateLocation()
    {
        transform.localPosition = Location.Position;
    }

    public virtual bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored && !cell.IsUnderwater && cell.units.Count == 0;
    }

    protected virtual bool IsValidCrossing(HexCell fromCell, HexCell toCell)
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
        IsMoving = true;
        location.units.Remove(this);
        location = path[^1];
        location.units.Add(this);
        pathToTravel = path;
        StartCoroutine(TravelPath());
    }

    protected virtual void FinishTravel(HexCell destination)
    {
    }

    private IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);
        grid.DecreaseVisibility(pathToTravel[0], visionRange);

        float t = Time.deltaTime * TravelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + pathToTravel[i].Position) * 0.5f;
            grid.IncreaseVisibility(pathToTravel[i], visionRange);
            for (; t < 1f; t += Time.deltaTime * TravelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            grid.DecreaseVisibility(pathToTravel[i], visionRange);
            t -= 1f;
        }

        a = c;
        b = location.Position;
        c = b;
        grid.IncreaseVisibility(location, visionRange);
        for (; t < 1f; t += Time.deltaTime * TravelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = location.Position;
        Orientation = transform.localRotation.eulerAngles.y;

        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        FinishTravel(location);
        IsMoving = false;
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
            orientation = transform.localRotation.eulerAngles.y;
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(owner);
        Location.coordinates.Save(writer);
        writer.Write(Orientation);
    }
    
    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        owner = reader.ReadInt32();
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(this, grid.GetCell(coordinates), orientation);
    }
}
