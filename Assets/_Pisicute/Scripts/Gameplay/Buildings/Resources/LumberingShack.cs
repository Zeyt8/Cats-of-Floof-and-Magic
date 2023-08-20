public class LumberingShack : Building
{
    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(GenerateWood);
    }

    private void GenerateWood()
    {
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(0, 5, 0, 0, 0, 0);
        }
    }
}
