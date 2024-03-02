using System;
using UnityEngine;

public class SunrayAbility : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private float damageModifier;

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
            int damage = (int)(caster.data.power.value * damageModifier);
            caster.DealDamage(cell.Unit, ref damage);
            EndTurn(caster);
        };
    }
}
