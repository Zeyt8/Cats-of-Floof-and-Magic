public class FluffyEffect : FactionEffect
{
    public override string Title => "Fluffy";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "Spells cost 1 floof less.";
            }
            return d;
        }
    }

    public FluffyEffect(Factions faction, int count) : base(faction, count)
    {
        if (count >= 2)
        {
            level = 1;
        }
        nextThreshold = 2;
    }
}
