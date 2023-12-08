public class ArmouredFactionStatusEffect : StatusEffect
{
    public override void OnTakeDamage(UnitObject self, UnitObject attacker, ref int damage)
    {
        base.OnTakeDamage(self, attacker, ref damage);
        damage -= 1;
    }
}
