public class AssassinEffect : FactionEffect
{
    public override string Title => "Assassin";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "Assassin cats deal extra damage to enemies under 50% health.";
            }
            return d;
        }
    }

    public AssassinEffect(Factions faction, int count) : base(faction, count)
    {
        if (count == 1)
        {
            level = 1;
        }
        nextThreshold = 1;
    }

    public override void Activate(UnitObject unit)
    {
        if (level == 0) return;
        base.Activate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Assassin))
        {
            unit.AddStatusEffect(new AssassinFactionStatusEffect());
        }
    }

    public override void Deactivate(UnitObject unit)
    {
        if (level == 0) return;
        base.Deactivate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Assassin))
        {
            unit.RemoveStatusEffect(typeof(AssassinFactionStatusEffect));
        }
    }
}
