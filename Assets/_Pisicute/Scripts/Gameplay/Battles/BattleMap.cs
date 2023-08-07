using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    public HexMapGenerator generator;
    [SerializeField] private CatCollection allCats;
    private HexGrid hexGrid;
    private List<Leader> battlingLeaders = new List<Leader>();
    private Cat currentCatTurn = null;
    private Queue<CatData> catsToPlace = new Queue<CatData>();
    private CatData currentCatToPlace = null;

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

    [ContextMenu("Generate Battle Map")]
    public void GenerateBattleMap(int width, int height, List<Leader> leaders)
    {
        battlingLeaders = leaders;
        generator.GenerateMap(width, height);
        foreach (HexGridChunk hgc in hexGrid.transform.GetComponentsInChildren<HexGridChunk>())
        {
            hgc.SetShaderCellVisibles(true);
        }
        SetupDeploy();
    }

    private void SetupDeploy()
    {
        foreach (Leader leader in battlingLeaders)
        {
            foreach (CatData cat in leader.army)
            {
                catsToPlace.Enqueue(cat);
            }
        }
        currentCatToPlace = catsToPlace.Dequeue();
        if (currentCatToPlace != null)
        {
            state = State.Deploy;
        }
        else
        {
            state = State.Fight;
        }
    }

    public void PlaceCat()
    {
        // place cat
        currentCatToPlace = catsToPlace.Dequeue();
        if (currentCatToPlace == null)
        {
            state = State.Fight;
        }
    }

    private void HighlightPlaceableTiles(CatData catData)
    {
        foreach (HexCell cell in hexGrid.cells)
        {
            if (allCats[catData.type].IsValidDestination(cell))
            {
                cell.EnableHighlight(Color.white);
            }
        }
    }
}
