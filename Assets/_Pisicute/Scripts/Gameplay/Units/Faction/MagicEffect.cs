public class MagicEffect : FactionEffect
{
    public override string Title => "Magic";
    public override string Description
    {
        get
        {
            string d = "";
            if (level >= 1)
            {
                d += "This leader regens 1 extra floof per turn.";
            }
            if (level >= 2)
            {
                d += "\nSpells have their cooldown reduced by 1 turn.";
            }
            return d;
        }
    }

    public MagicEffect(Factions faction, int count) : base(faction, count)
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
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new MagicFactionStatusEffect(-1, level), -1, leader.Location.coordinates, leader.owner);
    }

    public override void Deactivate(Leader leader)
    {
        if (level == 0) return;
        base.Deactivate(leader);
        PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(MagicFactionStatusEffect).ToString(), -1, leader.Location.coordinates, leader.owner);
    }
}
