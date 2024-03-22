using System;
using UnityEngine;

public class ConjureFoodSpell : Spell
{
    public override int floofCost => 5;
    public override int baseCooldown => 1;
    public override string description => "Conjure Food";

    public override PlayerObject.Action<HexCell> CastAbility(Leader caster, GameObject graphics)
    {
        return (cell) =>
        {
            caster.GainFloof(-floofCost);
            cooldown = baseCooldown;
            PlayerObject.Instance.CurrentResources += new Resources(2, 0, 0, 0, 0, 0);
            OnSpellCast(caster);
        };
    }

    public override Func<HexCell, bool> GetAvailableTargets(Leader caster)
    {
        return null;
    }
}
