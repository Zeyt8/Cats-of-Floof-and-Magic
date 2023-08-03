using System;

[Serializable]
public class Resources
{
    public int wood;
    public int stone;
    public int steel;
    public int sulfur;
    public int gems;

    public Resources() { }

    public Resources(int wood, int stone, int steel, int sulfur, int gems)
    {
        this.wood = wood;
        this.stone = stone;
        this.steel = steel;
        this.sulfur = sulfur;
        this.gems = gems;
    }

    public static Resources operator +(Resources a, Resources b)
    {
        Resources result = new Resources();
        result.wood = a.wood + b.wood;
        result.stone = a.stone + b.stone;
        result.steel = a.steel + b.steel;
        result.sulfur = a.sulfur + b.sulfur;
        result.gems = a.gems + b.gems;
        return result;
    }

    public static Resources operator -(Resources a, Resources b)
    {
        Resources result = new Resources();
        result.wood = a.wood - b.wood;
        result.stone = a.stone - b.stone;
        result.steel = a.steel - b.steel;
        result.sulfur = a.sulfur - b.sulfur;
        result.gems = a.gems - b.gems;
        return result;
    }

    public static bool operator >(Resources a, Resources b)
    {
        if (a.wood > b.wood && a.stone > b.stone && a.steel > b.steel && a.sulfur > b.sulfur && a.gems > b.gems)
            return true;
        else
            return false;
    }

    public static bool operator <(Resources a, Resources b)
    {
        if (a.wood < b.wood && a.stone < b.stone && a.steel < b.steel && a.sulfur < b.sulfur && a.gems < b.gems)
            return true;
        else
            return false;
    }

    public static bool operator ==(Resources a, Resources b)
    {
        if (a.wood == b.wood && a.stone == b.stone && a.steel == b.steel && a.sulfur == b.sulfur && a.gems == b.gems)
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
        if (a.wood >= b.wood && a.stone >= b.stone && a.steel >= b.steel && a.sulfur >= b.sulfur && a.gems >= b.gems)
            return true;
        else
            return false;
    }

    public static bool operator <=(Resources a, Resources b)
    {
        if (a.wood <= b.wood && a.stone <= b.stone && a.steel <= b.steel && a.sulfur <= b.sulfur && a.gems <= b.gems)
            return true;
        else
            return false;
    }

    public override bool Equals(object obj)
    {
        return this == (Resources)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + wood + " " + stone + " " + steel + " " + sulfur + gems + ")";
    }
}
