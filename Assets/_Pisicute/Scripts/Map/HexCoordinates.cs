using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates : ISaveableObject, INetworkSerializable, IEquatable<HexCoordinates>
{
    public int X => x;
    public int Z => z;
    public int Y => -X - Z;
    public float HexX => X + Z / 2 + ((Z % 2) == 0 ? 0f : 0.5f);
    public float HexZ => Z * HexMetrics.OuterToInner;

    [SerializeField] private int x, z;

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.InnerRadius * 2f);
        float y = -x;
        float offset = position.z / (HexMetrics.OuterRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }

    public int DistanceTo(HexCoordinates other)
    {
        return (Mathf.Abs(X - other.X) + Mathf.Abs(Y - other.Y) + Mathf.Abs(Z - other.Z)) / 2;
    }

    public override string ToString()
    {
        return "(" + X + ", " + Y + ", " + Z + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X + "\n" + Y + "\n" + Z;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(X);
        writer.Write(Z);
    }

    public void Load(BinaryReader reader, int header, HexGrid grid = null)
    {
    }

    public static HexCoordinates Load(BinaryReader reader)
    {
        HexCoordinates c;
        c.x = reader.ReadInt32();
        c.z = reader.ReadInt32();
        return c;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref z);
    }

    public bool Equals(HexCoordinates other)
    {
        return (x == other.x && z == other.z);
    }
}