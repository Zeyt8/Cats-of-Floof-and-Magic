public class FluffyFactionStatusEffect : StatusEffect
{
    public override string Name => "Fluffy Faction";
    public override string Description => "Casting a spell regenerates 1 floof.";

    public FluffyFactionStatusEffect(int duration, int level, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void OnSpellCast(Leader caster, Spell spell)
    {
        base.OnSpellCast(caster, spell);
        caster.GainFloof(1);
    }
}
