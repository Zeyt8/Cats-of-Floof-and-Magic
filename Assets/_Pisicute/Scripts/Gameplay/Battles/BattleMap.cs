using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

public class BattleMap : MonoBehaviour
{
    public HexMapGenerator generator;
    [HideInInspector] public HexGrid hexGrid;
    public int currentPlayer;
    [SerializeField] private CatCollection allCats;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CatHealthBar catHealthBarPrefab;
    private List<Leader> battlingLeaders = new List<Leader>();
    // deployment
    private Stack<Pair<int, CatData>> catsToPlace = new Stack<Pair<int, CatData>>();
    // battle
    private List<Cat> catTurnQueue = new List<Cat>();
    private List<Cat> nextCatTurnQueue = new List<Cat>();
    public Cat CurrentCatTurn => catTurnQueue[0];
    private List<Cat>[] armies = new List<Cat>[2];
    // grid dimensions
    private int width;
    private int height;

    private enum State
    {
        Deploy,
        Fight
    }
    private State state = State.Deploy;

    private void Awake()
    {
        hexGrid = GetComponent<HexGrid>();
    }

    private void Update()
    {
        if (state != State.Fight) return;
        for (int i = 0; i < armies.Length; i++)
        {
            if (armies[i].Count == 0)
            {
                FinishBattle(armies.Length - i, i + 1);
                break;
            }
        }
        for (int i = catTurnQueue.Count - 1; i >= 0; i--)
        {
            if (catTurnQueue[i] == null)
            {
                catTurnQueue.RemoveAt(i);
            }
        }
        for (int i = nextCatTurnQueue.Count - 1; i >= 0; i--)
        {
            if (nextCatTurnQueue[i] == null)
            {
                nextCatTurnQueue.RemoveAt(i);
            }
        }
    }

    public void OnWorldTurnEnd()
    {
        catTurnQueue = nextCatTurnQueue;
        nextCatTurnQueue = new List<Cat>();
        CurrentCatTurn.SetTurnActive(true);
        CurrentCatTurn.OnTurnStart(CurrentCatTurn.owner);
        currentPlayer = CurrentCatTurn.owner;
        BattleCanvas.Instance.SetTurnOrder(catTurnQueue);
    }

    public void GenerateBattleMap(int width, int height, List<Leader> leaders)
    {
        this.width = width;
        this.height = height;
        battlingLeaders = leaders;
        armies[0] = new List<Cat>();
        armies[1] = new List<Cat>();
        generator.GenerateMap(width, height);
        foreach (HexGridChunk hgc in hexGrid.transform.GetComponentsInChildren<HexGridChunk>())
        {
            hgc.SetShaderCellVisibles(true);
        }
        SetupDeploy();
    }

    public bool SelectedCell(HexCell cell)
    {
        if (state == State.Deploy)
        {
            PlaceCat(cell);
        }
        else if (state == State.Fight)
        {
        }
        return true;
    }

    public void SetBattleActive(bool active)
    {
        if (active)
        {
            virtualCamera.gameObject.SetActive(true);
        }
        else
        {
            virtualCamera.gameObject.SetActive(false);
        }
    }

    public List<Cat> GetOpponentArmy(int self)
    {
        if (self == 0)
        {
            return armies[battlingLeaders[1].owner - 1];
        }
        else
        {
            return armies[2 - self];
        }
    }

    private void SetupDeploy()
    {
        foreach (Leader leader in battlingLeaders)
        {
            foreach (CatData cat in leader.army)
            {
                catsToPlace.Push(new Pair<int, CatData>(leader.owner, cat));
            }
        }
        SetupNextCatPlacement();
    }

    private void PlaceCat(HexCell location)
    {
        // place cat
        Pair<int, CatData> catToPlace = catsToPlace.Peek();
        if (!IsCellPlaceable(catToPlace, location)) return;
        catToPlace = catsToPlace.Pop();
        Cat cat = InstantiateCat(allCats[catToPlace.item2.type], catToPlace.item1, location);
        cat.leader = GetLeader(cat.owner);
        SetupNextCatPlacement();
    }

    public Cat InstantiateCat(Cat cat, int owner, HexCell location)
    {
        Cat newCat = Instantiate(cat);
        newCat.owner = owner;
        newCat.battleMap = this;
        location.AddUnit(newCat, 0);
        catTurnQueue.Add(newCat);
        Instantiate(catHealthBarPrefab, BattleCanvas.Instance.transform).Initialize(newCat);
        if (newCat.owner == 0)
        {
            armies[2 - battlingLeaders[1].owner].Add(newCat);
        }
        else
        {
            armies[newCat.owner - 1].Add(newCat);
        }
        AudioManager.PlaySound(AudioLibrarySounds.SpawnCat);
        newCat.Orientation = 180;
        return newCat;
    }

    private void SetupNextCatPlacement()
    {
        bool catLeft = catsToPlace.TryPeek(out Pair<int, CatData> catToPlace);
        if (catLeft)
        {
            state = State.Deploy;
            HighlightPlaceableTiles(catToPlace);
            currentPlayer = catToPlace.item1;
            if (currentPlayer == 0)
            {
                List<HexCell> placeableCells = new List<HexCell>();
                foreach (HexCell cell in hexGrid.cells)
                {
                    if (IsCellPlaceable(catToPlace, cell))
                    {
                        placeableCells.Add(cell);
                    }
                }
                HexCell randomCell = placeableCells[Random.Range(0, placeableCells.Count)];
                PlaceCat(randomCell);
            }
        }
        else
        {
            SetupFight();
        }
    }

    private void HighlightPlaceableTiles(Pair<int, CatData> catData)
    {
        foreach (HexCell cell in hexGrid.cells)
        {
            if (IsCellPlaceable(catData, cell))
            {
                cell.EnableHighlight(HighlightType.Selection);
            }
            else
            {
                cell.DisableHighlight();
            }
        }
    }

    private bool IsCellPlaceable(Pair<int, CatData> catData, HexCell cell)
    {
        return allCats[catData.item2.type].IsValidDestination(cell) &&
                         ((catData.item1 == 1 && cell.coordinates.HexX < 2.5f) ||
                         (catData.item1 == 2 && cell.coordinates.HexX > width - 3) ||
                         (catData.item1 == 0 && cell.coordinates.HexX > width - 3));
    }

    private void UnhighlightAllTiles()
    {
        foreach (HexCell cell in hexGrid.cells)
        {
            cell.DisableHighlight();
        }
    }

    private void SetupFight()
    {
        foreach (Leader l in battlingLeaders)
        {
            if (l.owner == 0)
            {
                l.currentArmy = armies[2 - battlingLeaders[1].owner];
            }
            else
            {
                l.currentArmy = armies[l.owner - 1];
            }
        }
        state = State.Fight;
        UnhighlightAllTiles();
        catTurnQueue.Sort((cat1, cat2) => cat2.Speed - cat1.Speed);
        for (int i = 0; i < catTurnQueue.Count; i++)
        {
            if (catTurnQueue[i].data.factions.HasFlag(Factions.HiveMind))
            {
                int j = i + 1;
                while (j < catTurnQueue.Count && catTurnQueue[j].owner == catTurnQueue[i].owner)
                {
                    if (catTurnQueue[j].data.factions.HasFlag(Factions.HiveMind))
                    {
                        Cat temp = catTurnQueue[j];
                        catTurnQueue.RemoveAt(j);
                        catTurnQueue.Insert(i + 1, temp);
                        break;
                    }
                    j++;
                }
            }
        }
        currentPlayer = CurrentCatTurn.owner;
        CurrentCatTurn.SetTurnActive(true);
        foreach (Leader l in battlingLeaders)
        {
            l.RecalculateFactionEffects();
        }
        BattleCanvas.Instance.SetupFactionEffect(GetLeader(PlayerObject.Instance.playerNumber).factionsEffects);
        foreach (Cat cat in catTurnQueue)
        {
            cat.OnEncounterStart();
        }
        BattleCanvas.Instance.SetTurnOrder(catTurnQueue);
    }

    public void EndTurn()
    {
        CurrentCatTurn.SetTurnActive(false);
        nextCatTurnQueue.Add(CurrentCatTurn);
        catTurnQueue.RemoveAt(0);
        if (catTurnQueue.Count == 0)
        {
            BattleCanvas.Instance.ShowAbilities(null);
            BattleCanvas.Instance.SetTurnOrder(catTurnQueue);
            return;
        }
        if (CurrentCatTurn.owner == nextCatTurnQueue[^1].owner)
        {
            BattleCanvas.Instance.ShowAbilities(CurrentCatTurn);
        }
        else
        {
            BattleCanvas.Instance.ShowAbilities(null);
        }
        CurrentCatTurn.SetTurnActive(true);
        CurrentCatTurn.OnTurnStart(CurrentCatTurn.owner);
        currentPlayer = CurrentCatTurn.owner;
        BattleCanvas.Instance.SetTurnOrder(catTurnQueue);
    }

    private Leader GetLeader(int playerNumber)
    {
        foreach (Leader leader in battlingLeaders)
        {
            if (leader.owner == playerNumber)
            {
                return leader;
            }
        }
        return null;
    }

    private void FinishBattle(int winningPlayer, int losingPlayer)
    {
        GetLeader(losingPlayer).Die();
        LevelManager.Instance.GoToWorldMap();
        BattleManager.RemoveBattle(this);
        if (winningPlayer == PlayerObject.Instance.playerNumber)
        {
            AudioManager.PlaySound(AudioLibrarySounds.WinBattle);
        }
        else
        {
            //AudioManager.PlaySound(AudioLibrarySounds.LoseBattle);
        }
    }
}
