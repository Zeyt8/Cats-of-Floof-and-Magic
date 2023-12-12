public class SlowStatusEffect : StatusEffect
{
    public override string Name => "Bewitched";
    public override string Description => $"-{amount} Speed";

    private int amount;

    public SlowStatusEffect(int amount, int duration) : base(duration)
    {
        this.amount = amount;
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.speed.value -= amount;
    }
}
