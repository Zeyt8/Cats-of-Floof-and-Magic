using System;

public class CamouflageAbility : CatAbility
{
    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => false;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new CamouflageEffect(2), BattleManager.GetBattleMapIndex(caster.battleMap), caster.Location.coordinates, caster.owner);
        EndTurn(caster);
        return (cell) => { };
    }
}
