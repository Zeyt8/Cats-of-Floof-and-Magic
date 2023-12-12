public class ArmouredFactionStatusEffect : StatusEffect
{
    public ArmouredFactionStatusEffect(int duration): base(duration)
    {
    }

    public override void OnTakeDamage(UnitObject self, UnitObject attacker, ref int damage)
    {
        base.OnTakeDamage(self, attacker, ref damage);
        damage -= 1;
    }
}
