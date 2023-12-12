public class SICFactionStatusEffect : StatusEffect
{
    public SICFactionStatusEffect(int duration) : base(duration)
    {
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.power.value += 1;
    }
}
