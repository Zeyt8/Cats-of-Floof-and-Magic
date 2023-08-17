public class GemMine : Building
{
    public override void OnSpawn(HexCell cell)
    {
        GameManager.OnRoundEnd.AddListener(GenerateGems);
    }

    private void GenerateGems()
    {
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(0, 0, 0, 0, 0, 1);
        }
    }
}
