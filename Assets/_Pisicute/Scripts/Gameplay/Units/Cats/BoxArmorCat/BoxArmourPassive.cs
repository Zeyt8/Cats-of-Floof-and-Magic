using JSAM;

public class BoxArmourPassive : StatusEffect
{
    public override string Name => "Box Armour Cat Passive";
    public override string Description => "Gain 5 shield at the start of the encounter.";

    public BoxArmourPassive(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
    {
    }

    public override void OnEncounterStart(UnitObject unit)
    {
        base.OnEncounterStart(unit);
        unit.GainArmour(5);
        AudioManager.PlaySound(AudioLibrarySounds.Box);
    }
}
