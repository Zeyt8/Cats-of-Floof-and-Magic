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

    public override void Activate(Leader leader)
    {
        if (level == 0) return;
        base.Activate(leader);
        foreach (Cat cat in leader.currentArmy)
        {
            if (cat.data.factions.HasFlag(faction))
            {
                cat.AddStatusEffect(new SICFactionStatusEffect(-1));
            }
        }
    }

    public override void Deactivate(Leader leader)
    {
        if (level == 0) return;
        base.Deactivate(leader);
        foreach (Cat cat in leader.currentArmy)
        {
            if (cat.data.factions.HasFlag(faction))
            {
                cat.RemoveStatusEffect(typeof(SICFactionStatusEffect));
            }
        }
    }
}
