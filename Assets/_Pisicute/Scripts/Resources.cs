using System;

[Serializable]
public class Resources
{
    public int wood;
    public int stone;
    public int steel;
    public int sulfur;

    public Resources() { }

    public Resources(int wood, int stone, int steel, int sulfur)
    {
        this.wood = wood;
        this.stone = stone;
        this.steel = steel;
        this.sulfur = sulfur;
    }

    public static Resources operator +(Resources a, Resources b)
    {
        Resources result = new Resources();
        result.wood = a.wood + b.wood;
        result.stone = a.stone + b.stone;
        result.steel = a.steel + b.steel;
        result.sulfur = a.sulfur + b.sulfur;
        return result;
    }

    public static Resources operator -(Resources a, Resources b)
    {
        Resources result = new Resources();
        result.wood = a.wood - b.wood;
        result.stone = a.stone - b.stone;
        result.steel = a.steel - b.steel;
        result.sulfur = a.sulfur - b.sulfur;
        return result;
    }

    public static bool operator >(Resources a, Resources b)
    {
        if (a.wood > b.wood && a.stone > b.stone && a.steel > b.steel && a.sulfur > b.sulfur)
            return true;
        else
            return false;
    }

    public static bool operator <(Resources a, Resources b)
    {
        return !(a > b);
    }

    public static bool operator ==(Resources a, Resources b)
    {
        if (a.wood == b.wood && a.stone == b.stone && a.steel == b.steel && a.sulfur == b.sulfur)
            return true;
        else
            return false;
    }

    public static bool operator !=(Resources a, Resources b)
    {
        return !(a == b);
    }

    public static bool operator >=(Resources a, Resources b)
    {
        return (a > b || a == b);
    }

    public static bool operator <=(Resources a, Resources b)
    {
        return (a < b || a == b);
    }

    public override bool Equals(object obj)
    {
        return this == (Resources)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
