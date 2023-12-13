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

    public override void Activate(Leader leader)
    {
        if (level == 0) return;
        base.Activate(leader);
        foreach (Cat cat in leader.currentArmy)
        {
            if (cat.data.factions.HasFlag(faction))
            {
                cat.AddStatusEffect(new ArmouredFactionStatusEffect(-1));
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
                cat.RemoveStatusEffect(typeof(ArmouredFactionStatusEffect));
            }
        }
    }
}
