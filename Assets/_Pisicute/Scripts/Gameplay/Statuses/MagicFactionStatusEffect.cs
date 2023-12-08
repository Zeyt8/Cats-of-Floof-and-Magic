public class MagicFactionStatusEffect : StatusEffect
{
    private int level;

    public MagicFactionStatusEffect(int level)
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
