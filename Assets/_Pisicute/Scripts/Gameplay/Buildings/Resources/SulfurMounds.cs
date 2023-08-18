public class SulfurMounds : Building
{
    public override void OnSpawn(HexCell cell)
    {
        GameEvents.OnRoundEnd.AddListener(GenerateSulfur);
    }

    private void GenerateSulfur()
    {
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(0, 0, 0, 0, 1, 0);
        }
    }
}
