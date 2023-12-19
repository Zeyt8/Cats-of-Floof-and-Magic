public class TeleportingBoxPassive : StatusEffect
{
    public override string Name => "Teleporting Box Cat Passive";
    public override string Description => "Gain 2 shield at the start of the encounter.";

    public TeleportingBoxPassive(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void OnEncounterStart(UnitObject unit)
    {
        base.OnEncounterStart(unit);
        unit.GainArmour(2);
    }
}
