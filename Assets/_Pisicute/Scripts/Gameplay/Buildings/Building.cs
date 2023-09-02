using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Image playerMarker;

    public HexCell Location
    {
        get => location;
        set
        {
            location = value;
            if (PlayerObject.Instance && owner == PlayerObject.Instance.playerNumber)
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
        ChangeOwner(owner);
    }

    public virtual void OnUnitEnter(UnitObject unit)
    {
        ChangeOwner(unit.owner);
    }

    public virtual BuildingUI OpenUIPanel()
    {
        if (uiPanel == null) return null;
        BuildingUI buildingUI = Instantiate(uiPanel, GameManager.Instance.canvas.transform);
        buildingUI.Initialize(this);
        return buildingUI;
    }

    public void ChangeOwner(int player)
    {
        owner = player;
        playerMarker.color = PlayerColors.Get(player);
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
