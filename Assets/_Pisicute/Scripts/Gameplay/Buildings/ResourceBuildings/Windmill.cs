public class Windmill : Building
{
    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(GenerateFood);
    }

    public override void OnBuild(HexCell cell)
    {
        for (HexDirection i = HexDirection.NE; i < HexDirection.NW; i++)
        {
            cell.GetNeighbor(i).FarmLevel++;
        }
    }

    private void GenerateFood()
    {
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.CurrentResources += new Resources(5, 0, 0, 0, 0, 0);
        }
    }

    public override void OnUnitEnter(UnitObject unit)
    {
        if (owner != unit.owner)
        {
            if (unit.owner == PlayerObject.Instance.playerNumber)
            {
                PlayerObject.Instance.ResourceGain += new Resources(5, 0, 0, 0, 0, 0);
            }
            else
            {
                PlayerObject.Instance.ResourceGain -= new Resources(5, 0, 0, 0, 0, 0);
            }
        }
        base.OnUnitEnter(unit);
    }
}
