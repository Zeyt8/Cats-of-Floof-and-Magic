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

    public override void Activate(Leader leader)
    {
        if (level == 0) return;
        base.Activate(leader);
        foreach (Cat cat in leader.currentArmy)
        {
            if (cat.data.factions.HasFlag(faction))
            {
                PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new WildFactionStatusEffect(level, -1), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
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
                PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(WildFactionStatusEffect).ToString(), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
            }
        }
    }
}
