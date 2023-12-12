public class StatusEffect
{
    public int duration;
    public bool isInfinite;
    public virtual string Name => "";
    public virtual string Description => "";

    public StatusEffect(int duration)
    {
        this.duration = duration;
        isInfinite = (duration == -1);
    }

    public virtual void OnTurnBegin(UnitObject unit) { }
    public virtual void OnEncounterStart(UnitObject unit) { }
    public virtual int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell) { return 0; }
    public virtual void OnSpellCast() { }
    public virtual void OnDealDamage(UnitObject self, UnitObject target, ref int damage) { }
    public virtual void OnTakeDamage(UnitObject self, UnitObject attacker, ref int damage) { }
    public virtual void StatModifier(ref CatData data) { }
}
