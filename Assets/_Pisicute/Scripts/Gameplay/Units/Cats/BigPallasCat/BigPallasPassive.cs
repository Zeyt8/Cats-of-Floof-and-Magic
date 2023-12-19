public class BigPallasPassive : StatusEffect
{
    public override string Name => "Big Pallas Cat Passive";
    public override string Description => "Regen 1 health per turn.";

    public BigPallasPassive(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void OnTurnBegin(UnitObject unit)
    {
        base.OnTurnBegin(unit);
        unit.Heal(1);
    }
}
