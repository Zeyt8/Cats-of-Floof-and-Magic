public class BoxArmorCat : Cat
{
    public override void OnEncounterStart()
    {
        base.OnEncounterStart();
        AddStatusEffect(new BoxArmourPassive(-1));
    }
}
