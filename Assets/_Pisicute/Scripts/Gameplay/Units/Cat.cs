using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : UnitObject
{
    public CatData data;
    public override int Speed => data.speed;

    public override bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && cell.units.Count == 0;
    }
}
