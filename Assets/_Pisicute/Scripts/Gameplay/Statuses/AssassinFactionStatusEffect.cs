public class AssassinFactionStatusEffect : StatusEffect
{
    public override string Name => "Assassin Faction";
    public override string Description => "Deal 50% more damage to enemies under 50% health.";

    public AssassinFactionStatusEffect(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
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
