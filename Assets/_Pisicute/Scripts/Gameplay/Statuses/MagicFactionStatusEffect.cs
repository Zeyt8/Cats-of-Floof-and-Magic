public class MagicFactionStatusEffect : StatusEffect
{
    public override string Name => "Magic Faction";
    public override string Description => "Regenerate an additional 1 floof per turn." + (level >= 2 ? "\nSpells have their cooldown reduced by 1 turn." : "");

    private int level;

    public MagicFactionStatusEffect(int level, int duration) : base(duration)
    {
        this.level = level;
    }

    public override void OnTurnBegin(UnitObject unit)
    {
        base.OnTurnBegin(unit);
        ((Leader)unit).currentFloof += 1;
    }

    public override void OnSpellCast()
    {
        base.OnSpellCast();
        if (level >= 2)
        {
            // spell thing
        }
    }
}
