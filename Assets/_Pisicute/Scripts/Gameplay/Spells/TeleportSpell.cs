using System;
using UnityEngine;

public class TeleportSpell : Spell
{
    public override int floofCost => 5;
    public override int baseCooldown => 2;
    public override string description => "Teleport";

    public override PlayerObject.Action<HexCell> CastAbility(Leader caster, GameObject graphics)
    {
        return (cell) =>
        {
            GameObject.Instantiate(graphics, caster.transform.position, Quaternion.identity);
            caster.GainFloof(-floofCost);
            cooldown = baseCooldown;
            PlayerObject.Instance.TeleportUnitServerRpc(caster.Location.coordinates, cell.coordinates);
            OnSpellCast(caster);
            GameObject.Instantiate(graphics, cell.transform.position, Quaternion.identity);
        };
    }

    public override Func<HexCell, bool> GetAvailableTargets(Leader caster)
    {
        return (cell) => cell.Unit == null && caster.IsValidDestination(cell) && cell.coordinates.DistanceTo(caster.Location.coordinates) < 5;
    }
}
