using System;
using UnityEngine;

public class TeleportAbility : CatAbility
{
    [SerializeField] private int range;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            return !cell.Unit && cell.coordinates.DistanceTo(cat.Location.coordinates) <= range;
        };
    }
    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        base.CastAbility(caster);
        return (cell) =>
        {
            caster.Location = cell;
        };
    }
}
