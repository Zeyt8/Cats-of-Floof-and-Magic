public class WildEffect : FactionEffect
{
    public override string Title => "Wild";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "Wild cats gain the ability to camouflage.";
            }
            if (level >= 2)
            {
                d += "\nWild cats move faster in grasslands.";
            }
            return d;
        }
    }

    public WildEffect(Factions faction, int count) : base(faction, count)
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
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Wild))
        {
            unit.AddStatusEffect(new WildFactionStatusEffect(level, -1));
        }
    }

    public override void Deactivate(UnitObject unit)
    {
        if (level == 0) return;
        base.Deactivate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Wild))
        {
            unit.RemoveStatusEffect(typeof(WildFactionStatusEffect));
        }
    }
}
