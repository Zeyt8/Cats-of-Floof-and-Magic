using JSAM;

public class CatShelter : Building
{
    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.CatShelter);
    }

    public override BuildingUI OpenUIPanel()
    {
        if (Location.Unit == null || Location.Unit.owner != owner)
        {
            return null;
        }
        return base.OpenUIPanel();
    }
}
