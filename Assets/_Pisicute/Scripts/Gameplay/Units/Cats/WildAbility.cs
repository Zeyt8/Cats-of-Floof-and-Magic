using System;
using UnityEngine;

public class WildAbility : CatAbility
{
    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => false;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        // camouflage
        return (cell) => { };
    }
}
