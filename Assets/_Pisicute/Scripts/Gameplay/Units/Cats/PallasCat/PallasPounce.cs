using System;
using UnityEngine;

public class PallasPounce : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private float damageModifier;

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return (cell) =>
        {
            caster.Location = cell;
            int damage = (int)(caster.data.power.value * damageModifier);
            caster.DealDamage(cell.Unit, ref damage);
            EndTurn(caster);
        };
    }

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            return cell.Unit && cell.Unit.owner != cat.owner && cell.coordinates.DistanceTo(cat.Location.coordinates) <= range;
        };
    }
}
