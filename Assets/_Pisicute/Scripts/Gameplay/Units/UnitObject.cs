using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnitObject : MonoBehaviour, ISaveableObject
{
    public int owner;
    public Sprite icon;
    public virtual int Speed => 24;
    public int movementPoints;
    public bool IsMoving = false;
    [NonSerialized] public HexGrid grid;
    private const float TravelSpeed = 4f;
    public int visionRange = 3;

    private HexCell location;
    private float orientation;
    private List<HexCell> pathToTravel;
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

    private void OnEnable()
    {
        GameEvents.OnTurnStart.AddListener(ResetMovementPoints);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStart.RemoveListener(ResetMovementPoints);
    }

    protected virtual void Start()
    {
        movementPoints = Speed;
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
            moveCost = edgeType == HexEdgeType.Flat ? 4 : 6;
        }

        return moveCost;
    }

    public void Travel(List<HexCell> path)
    {
        IsMoving = true;
        pathToTravel = path;
        StartCoroutine(TravelPath());
    }

    protected virtual void FinishTravel(HexCell destination)
    {
    }

    private IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].Position;

        float t = Time.deltaTime * TravelSpeed;
        int i = 0;
        int nextMovementCost = GetMoveCost(pathToTravel[0], pathToTravel[1], pathToTravel[0].GetNeighborDirection(pathToTravel[1]));
        while (movementPoints - nextMovementCost > 0)
        {
            // move smoothly
            a = c;
            b = pathToTravel[i].Position;
            c = (b + pathToTravel[i + 1].Position) * 0.5f;
            grid.IncreaseVisibility(pathToTravel[i + 1], visionRange);
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
            // update cell data
            pathToTravel[i].units.Remove(this);
            pathToTravel[i + 1].units.Add(this);
            location = pathToTravel[i + 1];
            movementPoints -= nextMovementCost;
            i++;
            if (i >= pathToTravel.Count - 1)
            {
                break;
            }
            nextMovementCost = GetMoveCost(pathToTravel[i], pathToTravel[i + 1], pathToTravel[i].GetNeighborDirection(pathToTravel[i + 1]));
        }

        transform.localPosition = location.Position;
        Orientation = transform.localRotation.eulerAngles.y;
        Location = location;

        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        FinishTravel(location);
        IsMoving = false;
    }

    private void ResetMovementPoints(int player)
    {
        movementPoints = Speed;
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
