using JSAM;

public class TrainingGrounds : Building
{
    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.TrainingGrounds);
    }
}
