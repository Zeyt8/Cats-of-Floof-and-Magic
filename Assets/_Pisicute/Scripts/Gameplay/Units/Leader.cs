using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leader : UnitObject
{
    public int maxFloof;
    public int currentFloof;
    [SerializeField] private CatCollection sicCats;
    public List<CatData> army = new List<CatData>();

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
        AddCatToArmy(sicCats.cats.Values.GetRandom().data);
        AddCatToArmy(sicCats.cats.Values.GetRandom().data);
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
                List<Leader> leaders = new List<Leader>
                {
                    (Leader)destination.units[0],
                    (Leader)destination.units[1]
                };
                Random.State state = Random.state;
                Random.InitState(destination.coordinates.X + destination.coordinates.Y + destination.coordinates.Z + destination.Elevation + destination.index);
                destination.battleMap = BattleManager.Instance.GenerateBattle(destination.TerrainTypeIndex, leaders, Random.Range(0, int.MaxValue));
                Random.state = state;
                LevelManager.Instance.GoToBattleMap(destination.battleMap);
                break;
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
