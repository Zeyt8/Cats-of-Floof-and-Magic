public class HasteStatusEffect : StatusEffect
{
    public override string Name => "Haste";
    public override string Description => $"+{amount} Speed";

    public HasteStatusEffect(int amount, int duration) : base(duration, 0, amount)
    {
    }

    public override void StatModifier(ref CatData data)
    {
        base.StatModifier(ref data);
        data.speed.value += amount;
    }
}
