public class StoneQuarry : Building
{
    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(GenerateStone);
    }

    private void GenerateStone()
    {
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.CurrentResources += new Resources(0, 0, 5, 0, 0, 0);
        }
    }
}
