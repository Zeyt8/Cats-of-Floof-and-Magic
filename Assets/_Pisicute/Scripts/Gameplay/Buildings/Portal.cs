using JSAM;
using System.IO;
using UnityEngine;

public class Portal : Building
{
    [SerializeField] private HexCoordinates destination;

    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        action = () =>
        {
            if (PlayerObject.Instance.playerNumber == owner && Location.Unit)
            {
                PlayerObject.Instance.TeleportUnitServerRpc(Location.coordinates, destination);
            }
        };
    }

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);
        destination.Save(writer);
    }

    public override void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
        base.Load(reader, header, grid);
        destination = HexCoordinates.Load(reader);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.Portal);
    }

    public override BuildingUI OpenUIPanel()
    {
        if (Location.Unit == null || Location.Unit.owner != owner)
        {
            return null;
        }
        return base.OpenUIPanel();
    }
}
