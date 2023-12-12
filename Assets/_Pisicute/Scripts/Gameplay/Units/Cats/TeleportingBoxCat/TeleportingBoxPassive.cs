public class TeleportingBoxPassive : StatusEffect
{
    public override string Name => "Teleporting Box Cat Passive";
    public override string Description => "Gain 2 shield at the start of the encounter.";

    public TeleportingBoxPassive(int duration) : base(duration)
    {
    }

    public override void OnEncounterStart(UnitObject unit)
    {
        base.OnEncounterStart(unit);
        unit.GainArmour(2);
    }
}
