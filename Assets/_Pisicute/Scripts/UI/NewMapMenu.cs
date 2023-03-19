using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    [SerializeField] HexGrid _hexGrid;
    [SerializeField] HexMapGenerator _mapGenerator;
    private bool _generateMaps = true;

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
        if (_generateMaps)
        {
            _mapGenerator.GenerateMap(x, z);
        }
        else
        {
            _hexGrid.CreateMap(x, z);
        }
        gameObject.SetActive(false);
    }

    public void ToggleMapGeneration(bool toggle)
    {
        _generateMaps = toggle;
    }
}
