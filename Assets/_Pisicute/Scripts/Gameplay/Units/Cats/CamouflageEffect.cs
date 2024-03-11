public class CamouflageEffect : StatusEffect
{
    public override string Name => "Camouflaged";
    public override string Description => "Become immune to attacks that deal 1 or less damage until the end of the next turn or until you attack.\nThe first attack while camouflaged deals extra damage.";

    public CamouflageEffect(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void OnTakeDamage(UnitObject self, ref int damage)
    {
        base.OnTakeDamage(self, ref damage);
        if (damage == 1)
        {
            damage = 0;
        }
    }

    public override void OnDealDamage(UnitObject self, UnitObject target, ref int damage)
    {
        base.OnDealDamage(self, target, ref damage);
        damage *= 2;
        PlayerObject.Instance.RemoveStatusEffectFromUnitServerRpc(typeof(CamouflageEffect).ToString(), BattleManager.GetBattleMapIndex(((Cat)self).battleMap), self.Location.coordinates, self.owner);
    }

    public override void OnAdd(UnitObject unit)
    {
        base.OnAdd(unit);
        unit.Mesh.material.SetFloat("_Transparency", 0.5f);
    }

    public override void OnRemove(UnitObject unit)
    {
        base.OnRemove(unit);
        unit.Mesh.material.SetFloat("_Transparency", 1.0f);
    }
}
