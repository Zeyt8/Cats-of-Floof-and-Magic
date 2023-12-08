using System.Collections.Generic;
using System.Linq;

public class FactionEffect
{
    public int level { get; protected set; }
    public int count { get; protected set; }
    public int nextThreshold { get; protected set; }
    public Factions faction { get; private set; }
    public virtual string Title { get; }
    public virtual string Description { get; }

    public FactionEffect(Factions faction, int count)
    {
        level = 0;
        this.count = count;
        this.faction = faction;
    }

    public virtual void Activate(UnitObject unit) { }

    public virtual void Deactivate(UnitObject unit) { }

    public static Dictionary<Factions, int> CalculateFactions(List<CatData> cats)
    {
        Dictionary<Factions, int> currentFactions = new Dictionary<Factions, int>();
        foreach (CatData cat in cats)
        {
            foreach (Factions faction in Factions.GetValues(typeof(Factions)))
            {
                if (cat.factions.HasFlag(faction))
                {
                    currentFactions[faction] = currentFactions.GetValueOrDefault(faction, 0) + 1;
                }
            }
        }
        int wildcards = currentFactions.GetValueOrDefault(Factions.Wildcard, 0);
        currentFactions.Remove(Factions.Wildcard);
        List<Factions> keys = currentFactions.Keys.ToList();
        foreach (Factions f in keys)
        {
            currentFactions[f] += wildcards > 0 ? 1 : 0;
        }
        return currentFactions;
    }

    public static List<FactionEffect> CalculateFactionEffects(Dictionary<Factions, int> ownedFactions)
    {
        List<FactionEffect> effects = new List<FactionEffect>();
        foreach (Factions faction in ownedFactions.Keys)
        {
            switch (faction)
            {
                case Factions.Wild:
                    effects.Add(new WildEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.Magic:
                    effects.Add(new MagicEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.HiveMind:
                    effects.Add(new HiveMindEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.SIC:
                    effects.Add(new SICEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.Desert:
                    effects.Add(new DesertEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.Fluffy:
                    effects.Add(new FluffyEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.Assassin:
                    effects.Add(new AssassinEffect(faction, ownedFactions[faction]));
                    break;
                case Factions.Armoured:
                    effects.Add(new ArmouredEffect(faction, ownedFactions[faction]));
                    break;
            }
        }
        return effects;
    }
}
