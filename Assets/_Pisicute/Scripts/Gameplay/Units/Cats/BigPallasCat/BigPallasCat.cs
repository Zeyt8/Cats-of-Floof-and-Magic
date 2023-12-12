public class BigPallasCat : Cat
{
    public override void OnEncounterStart()
    {
        AddStatusEffect(new BigPallasPassive(-1));
        base.OnEncounterStart();
    }
}
