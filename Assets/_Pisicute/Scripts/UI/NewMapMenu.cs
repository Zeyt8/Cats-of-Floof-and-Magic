using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    [SerializeField] HexGrid hexGrid;
    [SerializeField] HexMapGenerator mapGenerator;
    private bool generateMaps = true;

    public void CreateSmallMap()
    {
        CreateMap(20, 15);
    }

    public void CreateMediumMap()
    {
        CreateMap(40, 30);
    }

    public void CreateLargeMap()
    {
        CreateMap(80, 60);
    }

    private void CreateMap(int x, int z)
    {
        if (generateMaps)
        {
            mapGenerator.GenerateMap(x, z);
        }
        else
        {
            hexGrid.CreateMap(x, z);
        }
        gameObject.SetActive(false);
    }

    public void ToggleMapGeneration(bool toggle)
    {
        generateMaps = toggle;
    }
}
