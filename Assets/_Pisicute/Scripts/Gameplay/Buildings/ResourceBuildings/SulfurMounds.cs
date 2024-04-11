using JSAM;

public class SulfurMounds : Building
{
    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(GenerateSulfur);
    }

    public override void OnBuild(HexCell cell)
    {
        base.OnBuild(cell);
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.ResourceGain += new Resources(0, 0, 0, 0, 1, 0);
        }
    }

    private void GenerateSulfur()
    {
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.CurrentResources += new Resources(0, 0, 0, 0, 1, 0);
        }
    }

    public override void OnUnitEnter(UnitObject unit)
    {
        if (owner != unit.owner)
        {
            if (unit.owner == PlayerObject.Instance.playerNumber)
            {
                PlayerObject.Instance.ResourceGain += new Resources(0, 0, 0, 0, 1, 0);
            }
            else if (owner == PlayerObject.Instance.playerNumber)
            {
                PlayerObject.Instance.ResourceGain -= new Resources(0, 0, 0, 0, 1, 0);
            }
        }
        base.OnUnitEnter(unit);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.Sulfur);
    }
}
