public class SICEffect : FactionEffect
{
    public override string Title => "SIC";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "SIC cats gain 1 power.";
            }
            return d;
        }
    }

    public SICEffect(Factions faction, int count) : base(faction, count)
    {
        if (count >= 4)
        {
            level = 1;
        }
        nextThreshold = 4;
    }

    public override void Activate(UnitObject unit)
    {
        if (level == 0) return;
        base.Activate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.SIC))
        {
            unit.AddStatusEffect(new SICFactionStatusEffect(-1));
        }
    }

    public override void Deactivate(UnitObject unit)
    {
        if (level == 0) return;
        base.Deactivate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.SIC))
        {
            unit.RemoveStatusEffect(typeof(SICFactionStatusEffect));
        }
    }
}
