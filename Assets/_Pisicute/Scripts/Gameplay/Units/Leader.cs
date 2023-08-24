using System.Collections.Generic;
using System.Linq;

public class Leader : UnitObject
{
    public List<CatData> army = new List<CatData>();
    public int maxFloof;
    public int currentFloof;

    protected override void Start()
    {
        base.Start();
        currentFloof = maxFloof / 2;
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.leaders.Add(this);
        }
        GameEvents.OnLeaderRecruited.Invoke(owner);
    }

    private void OnDestroy()
    {
        if (owner == Player.Instance.playerNumber)
        {
            Player.Instance.leaders.Remove(this);
        }
    }

    public void AddCatToArmy(CatData data)
    {
        if (army.Count < 8)
        {
            army.Add(data);
        }
    }

    protected override void FinishTravel(HexCell destination)
    {
        foreach (UnitObject unit in destination.units)
        {
            if (unit.owner != owner)
            {
                List<Leader> leaders = new List<Leader>();
                foreach (UnitObject u in destination.units)
                {
                    leaders.Add((Leader)u);
                }
                destination.battleMap = BattleManager.Instance.GenerateBattle(destination.TerrainTypeIndex, leaders);
                GameManager.Instance.GoToBattleMap(destination.battleMap);
            }
        }
        Player.Instance.SelectCell(destination);
    }

    public override bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored && !cell.IsUnderwater && cell.units.All(unit => unit.owner != owner);
    }

    public override void Die()
    {
        if (Location)
        {
            grid.DecreaseVisibility(Location, visionRange);
            Location.units.Remove(this);
        }
        Destroy(gameObject);
    }
}
