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
                PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new SICFactionStatusEffect(-1), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
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
                PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(SICFactionStatusEffect).ToString(), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
            }
        }
    }
}
