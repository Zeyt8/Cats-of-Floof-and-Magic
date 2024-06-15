using JSAM;

public class TrainingGrounds : Building
{
    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.TrainingGrounds);
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
