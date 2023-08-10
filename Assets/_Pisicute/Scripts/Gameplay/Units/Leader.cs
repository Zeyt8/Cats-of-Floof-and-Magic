using System.Collections.Generic;
using System.Linq;

public class Leader : UnitObject
{
    public List<CatData> army = new List<CatData>();

    public void AddCatToArmy(CatData data)
    {
        army.Add(data);
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
                Player.Instance.GoToBattleMap(destination.battleMap);
            }
        }
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
