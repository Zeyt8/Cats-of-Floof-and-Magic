public class StoneQuarry : Building
{
    public override void OnSpawn(HexCell cell)
    {
        GameEvents.OnRoundEnd.AddListener(GenerateStone);
    }

    private void GenerateStone()
    {
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(0, 0, 5, 0, 0, 0);
        }
    }
}
