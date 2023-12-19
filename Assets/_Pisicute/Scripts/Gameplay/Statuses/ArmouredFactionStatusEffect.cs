public class ArmouredFactionStatusEffect : StatusEffect
{
    public override string Name => "Armoured Faction";
    public override string Description => "Take 1 less damage from any source.";

    public ArmouredFactionStatusEffect(int duration, int level = 0, int amount = 0): base(duration, level, amount)
    {
    }

    public override void OnTakeDamage(UnitObject self, ref int damage)
    {
        base.OnTakeDamage(self, ref damage);
        damage -= 1;
    }
}
