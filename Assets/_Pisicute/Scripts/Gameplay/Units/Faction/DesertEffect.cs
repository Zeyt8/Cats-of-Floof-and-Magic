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

    public override void Activate(Leader leader)
    {
        if (level == 0) return;
        base.Activate(leader);
        foreach (Cat cat in leader.currentArmy)
        {
            if (cat.data.factions.HasFlag(faction))
            {
                PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new DesertFactionStatusEffect(-1), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
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
                PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(DesertFactionStatusEffect).ToString(), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
            }
        }
    }
}
