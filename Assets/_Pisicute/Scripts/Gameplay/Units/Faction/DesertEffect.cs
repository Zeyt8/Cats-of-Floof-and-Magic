public class DesertEffect : FactionEffect
{
    public override string Title => "Desert";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "Desert cats move faster on sand.";
            }
            return d;
        }
    }

    public DesertEffect(Factions faction, int count) : base(faction, count)
    {
        if (count >= 2)
        {
            level = 1;
        }
        nextThreshold = 2;
    }

    public override void Activate(UnitObject unit)
    {
        if (level == 0) return;
        base.Activate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Desert))
        {
            unit.AddStatusEffect(new DesertFactionStatusEffect());
        }
    }

    public override void Deactivate(UnitObject unit)
    {
        if (level == 0) return;
        base.Deactivate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Desert))
        {
            unit.RemoveStatusEffect(typeof(DesertFactionStatusEffect));
        }
    }
}
