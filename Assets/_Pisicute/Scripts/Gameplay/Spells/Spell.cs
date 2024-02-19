using System;

public abstract class Spell
{
    public virtual int floofCost => 0;
    public virtual int baseCooldown => 0;
    public virtual string description => "";
    public int cooldown;

    public abstract Func<HexCell, bool> GetAvailableTargets(Leader caster);

    public abstract PlayerObject.Action<HexCell> CastAbility(Leader caster);

    public void OnSpellCast(Leader caster)
    {
        foreach (StatusEffect statusEffect in caster.statusEffects)
        {
            statusEffect.OnSpellCast(caster, this);
        }
    }
}
