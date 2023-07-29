using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : UnitObject
{
    private List<CatData> army = new List<CatData>();

    public void AddCatToArmy(CatData data)
    {
        army.Add(data);
    }
}
