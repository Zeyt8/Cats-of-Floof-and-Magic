public class SlowStatusEffect : StatusEffect
{
    public override string Name => "Bewitched";
    public override string Description => $"-{amount} Speed";

    public SlowStatusEffect(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.speed.value -= amount;
    }
}
