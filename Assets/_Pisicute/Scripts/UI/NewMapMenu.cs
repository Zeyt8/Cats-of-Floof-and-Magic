using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    [SerializeField] HexGrid _hexGrid;

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
        _hexGrid.CreateMap(x, z);
        gameObject.SetActive(false);
    }
}
