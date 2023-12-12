public class BoxArmorCat : Cat
{
    public override void OnEncounterStart()
    {
        AddStatusEffect(new BoxArmourPassive(-1));
        base.OnEncounterStart();
    }
}
