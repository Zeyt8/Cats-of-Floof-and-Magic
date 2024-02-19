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
                d += "Casting a spell regenerates 1 floof.";
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

    public override void Activate(Leader leader)
    {
        if (level == 0) return;
        base.Activate(leader);
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new FluffyFactionStatusEffect(-1, level), -1, leader.Location.coordinates, leader.owner);
    }

    public override void Deactivate(Leader leader)
    {
        if (level == 0) return;
        base.Deactivate(leader);
        PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(FluffyFactionStatusEffect).ToString(), -1, leader.Location.coordinates, leader.owner);
    }
}
