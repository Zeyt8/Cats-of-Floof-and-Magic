public class MagicFactionStatusEffect : StatusEffect
{
    public override string Name => "Magic Faction";
    public override string Description => "Regenerate an additional 1 floof per turn." + (level >= 2 ? "\nSpells have their cooldown reduced by 1 turn." : "");

    public MagicFactionStatusEffect(int duration, int level, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void OnTurnBegin(UnitObject unit)
    {
        base.OnTurnBegin(unit);
        ((Leader)unit).currentFloof += 1;
    }

    public override void OnSpellCast(Leader caster, Spell spell)
    {
        base.OnSpellCast(caster, spell);
        if (level >= 2)
        {
            spell.cooldown -= 1;
            if (spell.cooldown < 0)
            {
                spell.cooldown = 0;
            }
        }
    }
}
