public class ArmouredFactionStatusEffect : StatusEffect
{
    public override string Name => "Armoured Faction";
    public override string Description => "Take 1 less damage from any source.";

    public ArmouredFactionStatusEffect(int duration): base(duration)
    {
    }

    public override void OnTakeDamage(UnitObject self, UnitObject attacker, ref int damage)
    {
        base.OnTakeDamage(self, attacker, ref damage);
        damage -= 1;
    }
}
