using System;

public class TeleportSpell : Spell
{
    public override int floofCost => 5;
    public override int baseCooldown => 2;

    public override PlayerObject.Action<HexCell> CastAbility(Leader caster)
    {
        return (cell) => caster.Location = cell;
    }

    public override Func<HexCell, bool> GetAvailableTargets(Leader caster)
    {
        return (cell) => cell.Unit == null && caster.IsValidDestination(cell) && cell.coordinates.DistanceTo(caster.Location.coordinates) < 10;
    }
}
