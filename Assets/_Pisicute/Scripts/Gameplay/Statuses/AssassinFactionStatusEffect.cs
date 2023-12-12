public class AssassinFactionStatusEffect : StatusEffect
{
    public AssassinFactionStatusEffect(int duration) : base(duration)
    {
    }

    public override void OnDealDamage(UnitObject self, UnitObject target, ref int damage)
    {
        base.OnDealDamage(self, target, ref damage);
        if (((Cat)target).data.health < ((Cat)target).data.maxHealth.value / 2)
        {
            damage += damage / 2;
        }
    }
}
