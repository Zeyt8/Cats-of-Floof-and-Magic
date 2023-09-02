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
}
