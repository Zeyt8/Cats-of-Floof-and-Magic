public class ArmouredEffect : FactionEffect
{
    public override string Title => "Armoured";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "Armoured cats take 1 less damage.";
            }
            return d;
        }
    }

    public ArmouredEffect(Factions faction, int count) : base(faction, count)
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
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Armoured))
        {
            unit.AddStatusEffect(new ArmouredFactionStatusEffect());
        }
    }

    public override void Deactivate(UnitObject unit)
    {
        if (level == 0) return;
        base.Deactivate(unit);
        if (unit is Cat && ((Cat)unit).data.factions.HasFlag(Factions.Armoured))
        {
            unit.RemoveStatusEffect(typeof(ArmouredFactionStatusEffect));
        }
    }
}
