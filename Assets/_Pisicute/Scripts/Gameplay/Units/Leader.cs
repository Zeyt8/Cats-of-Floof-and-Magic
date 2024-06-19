using JSAM;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Leader : UnitObject
{
    public int maxFloof;
    public int currentFloof;
    [SerializeField] protected CatCollection allCats;
    [SerializeField] private List<Sprite> possibleIcons = new List<Sprite>();
    public List<CatData> army = new List<CatData>();
    public List<Cat> currentArmy = new List<Cat>();
    public List<FactionEffect> factionsEffects { get; private set; }
    public List<Spell> spells = new List<Spell>();

    private void Awake()
    {
        factionsEffects = new List<FactionEffect>();
        icon = possibleIcons.GetRandom();
    }

    protected override void Start()
    {
        base.Start();
        currentFloof = maxFloof / 2;
        if (PlayerObject.Instance && owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.leaders.Add(this);
        }
        AddCatToArmy(allCats[CatTypes.TeleportingBox].data);
        AddCatToArmy(allCats[CatTypes.Tuxedo].data);
        GameEvents.OnLeaderRecruited.Invoke(owner);
        AddSpell(new ConjureFoodSpell());
    }

    private void OnEnable()
    {
        GameEvents.OnTurnStart.AddListener(OnTurnStart);
        GameEvents.OnRoundEnd.AddListener(OnRoundEnd);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStart.RemoveListener(OnTurnStart);
        GameEvents.OnRoundEnd.RemoveListener(OnRoundEnd);
    }

    private void OnDestroy()
    {
        if (PlayerObject.Instance && owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.leaders.Remove(this);
        }
    }

    public void AddCatToArmy(CatData data)
    {
        if (army.Count < 6)
        {
            army.Add(data);
            RecalculateFactionEffects();
        }
    }

    public override void Travel(List<HexCell> path)
    {
        base.Travel(path);
        if (owner == PlayerObject.Instance.playerNumber)
        {
            AudioManager.PlaySound(AudioLibrarySounds.Move);
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
        if (PlayerObject.Instance && owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.SelectCell(destination);
        }
    }

    public override bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored && !cell.IsUnderwater && cell.units.All(unit => unit.owner != owner);
    }

    [ContextMenu("Die")]
    public override void Die()
    {
        if (Location)
        {
            grid.DecreaseVisibility(Location, visionRange);
            Location.units.Remove(this);
        }
        Destroy(gameObject);
    }

    public void RecalculateFactionEffects()
    {
        foreach (FactionEffect factionEffect in factionsEffects)
        {
            factionEffect.Deactivate(this);
        }
        factionsEffects = FactionEffect.CalculateFactionEffects(FactionEffect.CalculateFactions(army));
        foreach (FactionEffect factionEffect in factionsEffects)
        {
            factionEffect.Activate(this);
        }
    }

    public void GainFloof(int gain)
    {
        currentFloof = Mathf.Min(currentFloof + gain, maxFloof);
    }

    public override void OnTurnStart(int player)
    {
        base.OnTurnStart(player);
        if (player == owner)
        {
            GainFloof(2);
        }
    }

    private void OnRoundEnd()
    {
        foreach (Spell spell in spells)
        {
            if (spell.cooldown > 0)
            {
                spell.cooldown--;
            }
        }
    }

    public void AddSpell(Spell spell)
    {
        spells.Add(spell);
    }
}
