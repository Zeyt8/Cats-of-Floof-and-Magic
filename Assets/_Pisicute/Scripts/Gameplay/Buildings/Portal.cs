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
}
