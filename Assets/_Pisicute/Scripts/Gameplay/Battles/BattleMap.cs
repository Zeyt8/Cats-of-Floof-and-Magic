using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    public HexMapGenerator generator;
    public HexGrid hexGrid;
    [SerializeField] private CatCollection allCats;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private List<Leader> battlingLeaders = new List<Leader>();
    // cat deployment
    private Stack<Pair<int, CatData>> catsToPlace = new Stack<Pair<int, CatData>>();
    private Pair<int, CatData> currentCatToPlace = null;
    // cat turn order
    private List<Cat> catTurnQueue = new List<Cat>();
    private Cat currentCatTurn = null;
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
            if (currentCatToPlace.item1 != Player.Instance.playerNumber) return false;
            PlaceCat(cell);
        }
        else if (state == State.Fight)
        {
            if (currentCatTurn.owner  != Player.Instance.playerNumber) return false;
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
        bool catLeft = catsToPlace.TryPop(out currentCatToPlace);
        if (catLeft)
        {
            state = State.Deploy;
            HighlightPlaceableTiles(currentCatToPlace);
        }
        else
        {
            SetupFight();
        }
    }

    private void PlaceCat(HexCell location)
    {
        // TODO: Check location valid
        // place cat
        Cat cat = Instantiate(allCats[currentCatToPlace.item2.type]);
        cat.owner = currentCatToPlace.item1;
        location.AddUnit(cat, 0);
        catTurnQueue.Add(cat);
        bool catLeft = catsToPlace.TryPop(out currentCatToPlace);
        if (catLeft)
        {
            HighlightPlaceableTiles(currentCatToPlace);
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
        currentCatToPlace = null;
        UnhighlightAllTiles();
        catTurnQueue.Sort((cat1, cat2) => cat2.Speed - cat1.Speed);
        currentCatTurn = catTurnQueue[0];
        catTurnQueue.RemoveAt(0);
        catTurnQueue.Add(currentCatTurn);
    }
}
