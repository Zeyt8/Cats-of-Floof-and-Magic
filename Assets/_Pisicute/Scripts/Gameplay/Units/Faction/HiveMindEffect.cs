public class HiveMindEffect : FactionEffect
{
    public override string Title => "Hivemind";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "All Hivemind cats activate at once.";
            }
            return d;
        }
    }

    public HiveMindEffect(Factions faction, int count) : base(faction, count)
    {
        if (count >= 1)
        {
            level = 1;
        }
        nextThreshold = 1;
    }
}
