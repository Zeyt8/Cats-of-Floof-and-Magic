public class TeleportingBoxPassive : StatusEffect
{
    public TeleportingBoxPassive(int duration) : base(duration)
    {
    }

    public override void OnEncounterStart(UnitObject unit)
    {
        base.OnEncounterStart(unit);
        unit.GainArmour(2);
    }
}
