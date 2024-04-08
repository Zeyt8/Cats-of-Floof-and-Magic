using JSAM;

public class SteelFoundry : Building
{
    public static readonly Resources SmeltCost = new Resources(0, 5, 5, 0, 0, 0);
    public static readonly Resources SmeltGain = new Resources(0, 0, 0, 4, 0, 0);

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.Steel);
    }
}
