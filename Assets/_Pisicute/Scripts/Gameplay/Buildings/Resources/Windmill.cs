public class Windmill : Building
{
    public override void OnSpawn(HexCell cell)
    {
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
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(5, 0, 0, 0, 0, 0);
        }
    }
}
