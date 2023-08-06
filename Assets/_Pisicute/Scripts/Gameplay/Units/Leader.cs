using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : UnitObject
{
    private List<CatData> army = new List<CatData>();

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
                BattleManager.Instance.GenerateBattle(destination.TerrainTypeIndex);
            }
        }
    }

    public override bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored && !cell.IsUnderwater;
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
