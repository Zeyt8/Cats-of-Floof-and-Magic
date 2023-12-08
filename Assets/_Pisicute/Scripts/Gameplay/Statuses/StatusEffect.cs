public class StatusEffect
{
    public virtual void OnTurnBegin(UnitObject unit) { }
    public virtual void OnEncounterStart(UnitObject unit) { }
    public virtual int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell) { return 0; }
    public virtual void OnSpellCast() { }
    public virtual void OnDealDamage(UnitObject self, UnitObject target, ref int damage) { }
    public virtual void OnTakeDamage(UnitObject self, UnitObject attacker, ref int damage) { }
}
