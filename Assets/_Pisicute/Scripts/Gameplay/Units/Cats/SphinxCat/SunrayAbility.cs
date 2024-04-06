using JSAM;
using System;
using UnityEngine;

public class SunrayAbility : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private float damageModifier;
    [SerializeField] private GameObject sunrayGraphics;

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
            Instantiate(sunrayGraphics, caster.transform.position + Vector3.up * 5, AbilityRotation(caster, cell.Unit));
            int damage = (int)(caster.data.power.value * damageModifier);
            caster.DealDamage(cell.Unit, ref damage);
            EndTurn(caster);
            AudioManager.PlaySound(AudioLibrarySounds.Laser);
        };
    }
}
