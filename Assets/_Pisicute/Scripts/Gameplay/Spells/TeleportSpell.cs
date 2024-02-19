using System;

public class TeleportSpell : Spell
{
    public override int floofCost => 5;
    public override int baseCooldown => 2;
    public override string description => "Teleport";

    public override PlayerObject.Action<HexCell> CastAbility(Leader caster)
    {
        return (cell) =>
        {
            caster.GainFloof(-floofCost);
            cooldown = baseCooldown;
            PlayerObject.Instance.TeleportUnitServerRpc(caster.Location.coordinates, cell.coordinates);
            OnSpellCast(caster);
        };
    }

    public override Func<HexCell, bool> GetAvailableTargets(Leader caster)
    {
        return (cell) => cell.Unit == null && caster.IsValidDestination(cell) && cell.coordinates.DistanceTo(caster.Location.coordinates) < 5;
    }
}
