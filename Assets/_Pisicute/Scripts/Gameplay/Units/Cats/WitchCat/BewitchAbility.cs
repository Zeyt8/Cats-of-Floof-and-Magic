using System;
using UnityEngine;

public class BewitchAbility : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private int slowAmount;
    [SerializeField] private int duration;
    [SerializeField] private GameObject bewitchGraphics;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            return cell.Unit && cell.Unit.owner != cat.owner && cell.coordinates.DistanceTo(cat.Location.coordinates) <= range;
        };
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return (cell) =>
        {
            PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new SlowStatusEffect(duration, amount: slowAmount), BattleManager.GetBattleMapIndex(((Cat)cell.Unit).battleMap), cell.coordinates, cell.Unit.owner);
            EndTurn(caster);
            Instantiate(bewitchGraphics, cell.transform.position, Quaternion.identity);
        };
    }
}
