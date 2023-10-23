using System.Collections.Generic;

public class FactionEffect
{
    public int level { get; private set; }
    public Factions faction { get; private set; }

    public virtual FactionEffect(Factions faction, int level)
    { 
        this.level = level;
        this.faction = faction;
    }

    public virtual void Activate() { }

    public virtual void Deactivate() { }

    public static Dictionary<Factions, int> CalculateFactions(List<Cat> cats)
    {
        Dictionary<Factions, int> currentFactions = new Dictionary<Factions, int>();
        foreach (Cat cat in cats)
        {
            foreach (Factions faction in Factions.GetValues(typeof(Factions)))
            {
                if (cat.data.factions.HasFlag(faction))
                {
                    currentFactions[faction] = currentFactions.GetValueOrDefault(faction, 0) + 1;
                }
            }
        }
        int wildcards = currentFactions.GetValueOrDefault(Factions.Wildcard, 0);
        currentFactions.Remove(Factions.Wildcard);
        foreach (Factions faction in currentFactions.Keys)
        {
            currentFactions[faction] += wildcards;
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
                case Factions.Swarm:
                    effects.Add(new SwarmEffect(faction, ownedFactions[faction]));
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
