using System;
using UnityEngine;

public class HasteAbility : CatAbility
{
    [SerializeField] private int speedIncrease;
    [SerializeField] private int duration;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => false;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        base.CastAbility(caster);
        caster.AddStatusEffect(new HasteStatusEffect(speedIncrease, duration));
        return (cell) => { };
    }
}
