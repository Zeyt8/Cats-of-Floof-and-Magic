public class SICFactionStatusEffect : StatusEffect
{
    public override void OnEncounterStart(UnitObject unit)
    {
        base.OnEncounterStart(unit);
        ((Cat)unit).data.power += 1;
    }
}
