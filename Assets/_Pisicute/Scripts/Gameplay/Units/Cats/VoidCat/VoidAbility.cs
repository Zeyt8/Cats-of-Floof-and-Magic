using System;
using UnityEngine;

public class VoidAbility : CatAbility
{
    [SerializeField] private int floofRestored;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => false;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        base.CastAbility(caster);
        caster.leader.GainFloof(floofRestored);
        return (cell) => { };
    }
}
