using System.IO;

public interface ISaveableObject
{
    public void Save(BinaryWriter writer);

    public void Load(BinaryReader reader, int header, HexGrid grid = null);
}
