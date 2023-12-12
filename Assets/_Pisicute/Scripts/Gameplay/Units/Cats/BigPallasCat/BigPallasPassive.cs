public class BigPallasPassive : StatusEffect
{
    public BigPallasPassive(int duration) : base(duration)
    {
    }

    public override void OnTurnBegin(UnitObject unit)
    {
        base.OnTurnBegin(unit);
        unit.Heal(1);
    }
}
