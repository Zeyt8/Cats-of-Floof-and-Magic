public class OrientalCat : Cat
{
    public override void OnEncounterStart()
    {
        base.OnEncounterStart();
        AddStatusEffect(new OrientalPassive(-1));
    }
}
