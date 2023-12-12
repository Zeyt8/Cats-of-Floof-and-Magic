public class OrientalCat : Cat
{
    public override void OnEncounterStart()
    {
        AddStatusEffect(new OrientalPassive(-1));
        base.OnEncounterStart();
    }
}
