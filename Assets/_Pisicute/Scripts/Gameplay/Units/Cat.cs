using System.Collections.Generic;
using UnityEngine;

public class Cat : UnitObject
{
    public CatData data;
    public Resources sellCost;
    public override int Speed => data.speed;
    public List<CatAbility> abilities;
    [HideInInspector] public BattleMap battleMap;

    public override bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && cell.units.Count == 0;
    }

    public virtual void OnAbilityCasted(CatAbility abiltiy)
    {
        battleMap.EndTurn();
    }
}
