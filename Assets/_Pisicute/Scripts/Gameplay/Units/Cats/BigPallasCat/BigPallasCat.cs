public class BigPallasCat : Cat
{
    public override void OnEncounterStart()
    {
        base.OnEncounterStart();
        AddStatusEffect(new BigPallasPassive(-1));
    }
}
