using System;
using UnityEngine;

[Serializable]
public class HexFeatureCollection
{
    public GameObject[] Prefabs;
    public GameObject Pick(float choice)
    {
        return Prefabs[(int)choice * Prefabs.Length];
    }
}
