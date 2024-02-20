using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour, ISaveableObject
{
    public BuildingTypes type;
    public Resources resourceCost;
    public Sprite icon;
    [TextArea]
    public string title;
    [TextArea]
    public string description;
    public int visionRange = 3;
    [HideInInspector] public HexGridChunk chunk;
    public int owner = -1;
    public bool HasUIPanel => uiPanel != null;
    public bool HasAction => action != null;
    public Action action;
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
                chunk.grid.IncreaseVisibility(location, visionRange);
            }
            transform.position = location.Position;
        }
    }
    private HexCell location;

    private void Update()
    {
        if (playerMarker != null)
        {
            playerMarker.gameObject.SetActive(Location.IsExplored);
        }
    }

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
        BuildingUI buildingUI = Instantiate(uiPanel, LevelManager.Instance.canvas.transform);
        buildingUI.Initialize(this);
        return buildingUI;
    }

    public void ChangeOwner(int player, bool force = false)
    {
        if (owner == player && !force) return;
        if (!playerMarker) return;
        playerMarker.color = PlayerColors.Get(player);
        if (PlayerObject.Instance && player == PlayerObject.Instance.playerNumber)
        {
            chunk.grid.IncreaseVisibility(Location, visionRange);
        }
        else if (owner != -1)
        {
            chunk.grid.DecreaseVisibility(Location, visionRange);
        }
        owner = player;
    }

    public virtual void Save(BinaryWriter writer)
    {
        writer.Write((int)type); // this must be first
        writer.Write(owner);
        Location.coordinates.Save(writer);
    }

    public virtual void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        int o = reader.ReadInt32();
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        HexCell cell = grid.GetCell(coordinates);
        chunk = cell.chunk;
        chunk.AddBuilding(this, cell);
        ChangeOwner(o, true);
    }
}
