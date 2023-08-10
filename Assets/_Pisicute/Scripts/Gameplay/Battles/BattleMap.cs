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
    private Queue<CatData> catTurnQueue = new Queue<CatData>();
    private Cat currentCatTurn = null;
    private CatData currentCatToPlace = null;
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

    public void SelectedCell(HexCell cell)
    {
        if (state == State.Deploy)
        {
            PlaceCat(cell);
        }
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
                catTurnQueue.Enqueue(cat);
            }
        }
        bool catLeft = catTurnQueue.TryDequeue(out currentCatToPlace);
        if (catLeft)
        {
            state = State.Deploy;
            HighlightPlaceableTiles(currentCatToPlace);
        }
        else
        {
            state = State.Fight;
            currentCatToPlace = null;
        }
    }

    private void PlaceCat(HexCell location)
    {
        // place cat
        hexGrid.AddUnit(Instantiate(allCats[currentCatToPlace.type]), location, 0);
        bool catLeft = catTurnQueue.TryDequeue(out currentCatToPlace);
        if (catLeft)
        {
            HighlightPlaceableTiles(currentCatToPlace);
        }
        else
        {
            state = State.Fight;
            currentCatToPlace = null;
        }
    }

    private void HighlightPlaceableTiles(CatData catData)
    {
        foreach (HexCell cell in hexGrid.cells)
        {
            if (allCats[catData.type].IsValidDestination(cell) && (cell.coordinates.HexX < 2.5f || cell.coordinates.HexX > width - 3))
            {
                cell.EnableHighlight(Color.white);
            }
        }
    }
}
