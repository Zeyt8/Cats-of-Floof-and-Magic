public class TeleportingBoxCat : Cat
{
    public override void OnEncounterStart()
    {
        base.OnEncounterStart();
        AddStatusEffect(new TeleportingBoxPassive(-1));
    }
}
