using JSAM;

public class CatShelter : Building
{
    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.CatShelter);
    }
}
