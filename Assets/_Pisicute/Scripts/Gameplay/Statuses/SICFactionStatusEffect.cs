public class SICFactionStatusEffect : StatusEffect
{
    public override string Name => "SIC Faction";
    public override string Description => "+1 Power";

    public SICFactionStatusEffect(int duration) : base(duration)
    {
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.power.value += 1;
    }
}
