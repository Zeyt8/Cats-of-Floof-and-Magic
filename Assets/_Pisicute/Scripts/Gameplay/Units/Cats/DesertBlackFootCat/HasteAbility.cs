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
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new HasteStatusEffect(duration, 0, speedIncrease), BattleManager.GetBattleMapIndex(caster.battleMap), caster.Location.coordinates, caster.owner);
        EndTurn(caster);
        return (cell) => { };
    }
}
