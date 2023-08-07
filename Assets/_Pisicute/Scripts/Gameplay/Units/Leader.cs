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
                BattleManager.Instance.GenerateBattle(destination.TerrainTypeIndex, destination.units);
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
