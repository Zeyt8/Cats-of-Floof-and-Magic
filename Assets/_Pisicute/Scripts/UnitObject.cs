using System.IO;
using UnityEngine;

public class UnitObject : MonoBehaviour, ISaveableObject
{
    private HexCell _location;
    private float _orientation;
    public HexCell Location
    {
        get => _location;
        set
        {
            if (_location)
            {
                _location.Unit = null;
            }
            _location = value;
            value.Unit = this;
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
        Location.Unit = null;
        Destroy(gameObject);
    }

    public void ValidateLocation()
    {
        transform.localPosition = Location.Position;
    }

    public bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && !cell.Unit;
    }

    public bool IsValidCrossing(HexCell previous, HexCell next)
    {
        HexEdgeType edgeType = previous.GetEdgeType(next);
        return edgeType != HexEdgeType.Cliff;
    }

    public void Save(BinaryWriter writer)
    {
        Location.Coordinates.Save(writer);
        writer.Write(Orientation);
    }
    
    public void Load(BinaryReader reader, int header = -1, HexGrid grid = null)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(this, grid.GetCell(coordinates), orientation);
    }
}
