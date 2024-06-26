using JSAM;
using System;
using UnityEngine;

public class HasteAbility : CatAbility
{
    [SerializeField] private int speedIncrease;
    [SerializeField] private int duration;
    [SerializeField] private GameObject hasteGraphics;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => cell == cat.Location;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        Instantiate(hasteGraphics, caster.transform.position, Quaternion.identity);
        AudioManager.PlaySound(AudioLibrarySounds.Haste);
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new HasteStatusEffect(duration, 0, speedIncrease), BattleManager.GetBattleMapIndex(caster.battleMap), caster.Location.coordinates, caster.owner);
        EndTurn(caster);
        return (cell) => { };
    }
}
