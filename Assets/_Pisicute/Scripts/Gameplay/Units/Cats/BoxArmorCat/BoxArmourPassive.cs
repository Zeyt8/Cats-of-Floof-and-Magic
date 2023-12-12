public class BoxArmourPassive : StatusEffect
{
    public BoxArmourPassive(int duration) : base(duration)
    {
    }

    public override void OnEncounterStart(UnitObject unit)
    {
        base.OnEncounterStart(unit);
        unit.GainArmour(5);
    }
}
