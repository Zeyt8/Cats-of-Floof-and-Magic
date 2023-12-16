public class AssassinEffect : FactionEffect
{
    public override string Title => "Assassin";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "Assassin cats deal extra damage to enemies under 50% health.";
            }
            return d;
        }
    }

    public AssassinEffect(Factions faction, int count) : base(faction, count)
    {
        if (count == 1)
        {
            level = 1;
        }
        nextThreshold = 1;
    }

    public override void Activate(Leader leader)
    {
        if (level == 0) return;
        base.Activate(leader);
        foreach (Cat cat in leader.currentArmy)
        {
            if (cat.data.factions.HasFlag(faction))
            {
                PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new AssassinFactionStatusEffect(-1), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
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
                PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(AssassinFactionStatusEffect).ToString(), BattleManager.GetBattleMapIndex(cat.battleMap), cat.Location.coordinates, cat.owner);
            }
        }
    }
}
