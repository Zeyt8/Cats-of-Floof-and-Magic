using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    public HexMapGenerator generator;
    public HexGrid hexGrid;
    public int currentPlayer;
    [SerializeField] private CatCollection allCats;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private List<Leader> battlingLeaders = new List<Leader>();
    // deployment
    private Stack<Pair<int, CatData>> catsToPlace = new Stack<Pair<int, CatData>>();
    // battle
    private List<Cat> catTurnQueue = new List<Cat>();
    private Cat CurrentCatTurn => catTurnQueue[0];
    private List<Cat>[] armies = new List<Cat>[2];
    private List<FactionEffect> selfFactions = new List<FactionEffect>();
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
        if (currentPlayer != PlayerObject.Instance.playerNumber) return false;
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
        // TODO: Check location valid
        // place cat
        Pair<int, CatData> catToPlace = catsToPlace.Pop();
        Cat cat = Instantiate(allCats[catToPlace.item2.type]);
        cat.owner = catToPlace.item1;
        cat.battleMap = this;
        location.AddUnit(cat, 0);
        catTurnQueue.Add(cat);
        armies[cat.owner - 1].Add(cat);
        SetupNextCatPlacement();
    }

    private void SetupNextCatPlacement()
    {
        bool catLeft = catsToPlace.TryPeek(out Pair<int, CatData> catToPlace);
        if (catLeft)
        {
            state = State.Deploy;
            HighlightPlaceableTiles(catToPlace);
            currentPlayer = catToPlace.item1;
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
            bool condition = allCats[catData.item2.type].IsValidDestination(cell) &&
                ((catData.item1 == 1 && cell.coordinates.HexX < 2.5f) || (catData.item1 == 2 && cell.coordinates.HexX > width - 3));
            if (condition)
            {
                cell.EnableHighlight(HighlightType.Selection);
            }
            else
            {
                cell.DisableHighlight();
            }
        }
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
        state = State.Fight;
        UnhighlightAllTiles();
        catTurnQueue.Sort((cat1, cat2) => cat2.Speed - cat1.Speed);
        currentPlayer = CurrentCatTurn.owner;
        BattleCanvas.Instance.ShowAbilities(CurrentCatTurn);
        selfFactions = FactionEffect.CalculateFactionEffects(FactionEffect.CalculateFactions(armies[PlayerObject.Instance.playerNumber - 1]));
        BattleCanvas.Instance.SetupFactionEffect(selfFactions);
    }

    public void EndTurn()
    {
        catTurnQueue.Add(CurrentCatTurn);
        catTurnQueue.RemoveAt(0);
        BattleCanvas.Instance.ShowAbilities(CurrentCatTurn);
    }
}
