using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : UnitObject
{
    public CatData data;
    public override int Speed => data.speed;
}
