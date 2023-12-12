public class OrientalPassive : StatusEffect
{
    public OrientalPassive(int duration) : base(duration)
    {
    }

    public override void OnDealDamage(UnitObject self, UnitObject target, ref int damage)
    {
        base.OnDealDamage(self, target, ref damage);
        if (target.Location.TerrainTypeIndex == 0)
        {
            damage += (int)(damage * 0.3f);
        }
    }
}
