using JSAM;

public class Catapult : Building
{
    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        OnRoundEnd();
        GameEvents.OnRoundEnd.AddListener(OnRoundEnd);
    }

    private bool ValidTargets(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (cell.HasWallThroughEdge(d))
            {
                return true;
            }
        }
        return false;
    }

    private void Shoot(HexCell cell)
    {
        cell.RemoveWall();
    }

    private void OnRoundEnd()
    {
        action = () =>
        {
            PlayerObject.Instance.InitiateSelectCellForEffect(ValidTargets, Shoot);
            action = null;
        };
    }

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.Catapult);
    }
}
