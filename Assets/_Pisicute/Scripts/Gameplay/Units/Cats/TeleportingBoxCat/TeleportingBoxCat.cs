public class TeleportingBoxCat : Cat
{
    public override void OnEncounterStart()
    {
        AddStatusEffect(new TeleportingBoxPassive(-1));
        base.OnEncounterStart();
    }
}
