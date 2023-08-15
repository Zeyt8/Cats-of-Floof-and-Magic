public class LumberingShack : Building
{
    private void OnEnable()
    {
        GameManager.Instance.OnTurnEnd.AddListener(GenerateWood);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTurnEnd.RemoveListener(GenerateWood);
    }

    private void GenerateWood(int player)
    {
        if (player == owner && owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(0, 5, 0, 0, 0, 0);
        }
    }
}
