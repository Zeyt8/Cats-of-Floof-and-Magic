public class HasteStatusEffect : StatusEffect
{
    private int amount;

    public HasteStatusEffect(int amount, int duration) : base(duration)
    {
        this.amount = amount;
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.speed.value += amount;
    }
}
