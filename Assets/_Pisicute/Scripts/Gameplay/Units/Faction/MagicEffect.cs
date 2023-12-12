public class MagicEffect : FactionEffect
{
    public override string Title => "Magic";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "This leader regens 1 extra floof per turn.";
            }
            if (level >= 2)
            {
                d += "\nSpells have their cooldown reduced by 1 turn.";
            }
            return d;
        }
    }

    public MagicEffect(Factions faction, int count) : base(faction, count)
    {
        if (count >= 4)
        {
            level = 2;
            nextThreshold = 4;
        }
        else if (count >= 2)
        {
            level = 1;
            nextThreshold = 4;
        }
        else
        {
            nextThreshold = 2;
        }
    }

    public override void Activate(UnitObject unit)
    {
        if (level == 0) return;
        base.Activate(unit);
        if (unit is Leader)
        {
            unit.AddStatusEffect(new MagicFactionStatusEffect(level, -1));
        }
    }

    public override void Deactivate(UnitObject unit)
    {
        if (level == 0) return;
        base.Deactivate(unit);
        if (unit is Leader)
        {
            unit.RemoveStatusEffect(typeof(MagicFactionStatusEffect));
        }
    }
}
