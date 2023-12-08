public class AssassinFactionStatusEffect : StatusEffect
{
    public override void OnDealDamage(UnitObject self, UnitObject target, ref int damage)
    {
        base.OnDealDamage(self, target, ref damage);
        if (((Cat)target).data.health < ((Cat)target).data.maxHealth / 2)
        {
            damage += damage / 2;
        }
    }
}
