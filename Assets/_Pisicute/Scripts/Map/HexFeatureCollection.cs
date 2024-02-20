using System;
using UnityEngine;

[Serializable]
public class HexFeatureCollection
{
    public GameObject[] prefabs;
    public GameObject Pick(float choice)
    {
        return prefabs[(int)(choice * prefabs.Length)];
    }
}
