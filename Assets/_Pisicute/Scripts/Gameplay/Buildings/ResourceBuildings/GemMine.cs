using JSAM;

public class GemMine : Building
{
    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(GenerateGems);
    }

    private void GenerateGems()
    {
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.CurrentResources += new Resources(0, 0, 0, 0, 0, 1);
        }
    }

    public override void OnUnitEnter(UnitObject unit)
    {
        if (owner != unit.owner)
        {
            if (unit.owner == PlayerObject.Instance.playerNumber)
            {
                PlayerObject.Instance.ResourceGain += new Resources(0, 0, 0, 0, 0, 1);
            }
            else if (owner == PlayerObject.Instance.playerNumber)
            {
                PlayerObject.Instance.ResourceGain -= new Resources(0, 0, 0, 0, 0, 1);
            }
        }
        base.OnUnitEnter(unit);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.Gems);
    }
}
