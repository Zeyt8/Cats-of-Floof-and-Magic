using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources
{
    private int wood;
    private int stone;
    private int steel;
    private int sulfur;

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
}
