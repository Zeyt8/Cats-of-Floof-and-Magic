using System;
using UnityEngine;

public class BewitchAbility : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private int slowAmount;
    [SerializeField] private int duration;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            return cell.Unit && cell.Unit.owner != cat.owner && cell.coordinates.DistanceTo(cat.Location.coordinates) <= range;
        };
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        base.CastAbility(caster);
        return (cell) =>
        {
            cell.Unit.AddStatusEffect(new SlowStatusEffect(slowAmount, duration));
        };
    }
}
