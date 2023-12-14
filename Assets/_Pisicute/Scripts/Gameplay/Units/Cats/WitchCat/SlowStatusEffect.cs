public class SlowStatusEffect : StatusEffect
{
    public override string Name => "Bewitched";
    public override string Description => $"-{amount} Speed";

    public SlowStatusEffect(int amount, int duration) : base(duration, 0, amount)
    {
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.speed.value -= amount;
    }
}
