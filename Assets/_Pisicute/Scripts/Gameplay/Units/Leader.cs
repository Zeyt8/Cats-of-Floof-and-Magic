using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leader : UnitObject
{
    public int maxFloof;
    public int currentFloof;
    [SerializeField] private CatCollection sicCats;
    [HideInInspector] public List<CatData> army = new List<CatData>();

    protected override void Start()
    {
        base.Start();
        currentFloof = maxFloof / 2;
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.leaders.Add(this);
        }
        GameEvents.OnLeaderRecruited.Invoke(owner);
        AddCatToArmy(sicCats.cats.Values.GetRandom().data);
    }

    private void OnDestroy()
    {
        if (owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.leaders.Remove(this);
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
        PlayerObject.Instance.SelectCell(destination);
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
