using System.IO;
using UnityEngine;

public class Building : MonoBehaviour, ISaveableObject
{
    public BuildingTypes type;
    public Resources resourceCost;
    public Sprite icon;
    [TextArea]
    public string description;
    public int visionRange = 3;
    [HideInInspector] public HexGrid grid;
    [HideInInspector] public int owner = -1;

    [HideInInspector]
    public HexCell Location
    {
        get => location;
        set
        {
            location = value;
            grid.IncreaseVisibility(location, visionRange);
            transform.position = location.Position;
        }
    }
    private HexCell location;

    public virtual void OnBuild(HexCell cell)
    {
    }

    public virtual void OnUnitEnter(UnitObject unit)
    {
        owner = unit.owner;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((int)type);
        Location.coordinates.Save(writer);
    }

    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        grid.AddBuilding(this, grid.GetCell(coordinates));
    }
}
