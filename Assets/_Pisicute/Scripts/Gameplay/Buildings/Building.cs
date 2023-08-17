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
    public int owner = -1;
    public bool HasUIPanel => uiPanel != null;
    [SerializeField] private BuildingUI uiPanel;

    public HexCell Location
    {
        get => location;
        set
        {
            location = value;
            if (Player.Instance && owner == Player.Instance.playerNumber)
            {
                grid.IncreaseVisibility(location, visionRange);
            }
            transform.position = location.Position;
        }
    }
    private HexCell location;

    public virtual void OnBuild(HexCell cell)
    {
    }

    public virtual void OnSpawn(HexCell cell)
    {
    }

    public virtual void OnUnitEnter(UnitObject unit)
    {
        owner = unit.owner;
    }

    public virtual BuildingUI OpenUIPanel()
    {
        if (uiPanel == null) return null;
        BuildingUI buildingUI = Instantiate(uiPanel, GameManager.Instance.canvas.transform);
        buildingUI.Initialize(this);
        return buildingUI;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((int)type); // this must be first
        writer.Write(owner);
        Location.coordinates.Save(writer);
    }

    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        owner = reader.ReadInt32();
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        grid.AddBuilding(this, grid.GetCell(coordinates));
    }
}
