public class ArmouredFactionStatusEffect : StatusEffect
{
    public override string Name => "Armoured Faction";
    public override string Description => "Take 1 less damage from any source.";

    public ArmouredFactionStatusEffect(int duration): base(duration)
    {
    }

    public override void OnTakeDamage(UnitObject self, ref int damage)
    {
        base.OnTakeDamage(self, ref damage);
        damage -= 1;
    }
}
