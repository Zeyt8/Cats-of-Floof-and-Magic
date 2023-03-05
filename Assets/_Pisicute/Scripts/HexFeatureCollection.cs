using UnityEngine;

[System.Serializable]
public class HexFeatureCollection
{
    [SerializeField] private Transform[] _prefabs;

    public Transform Pick(float choice)
    {
        return _prefabs[(int)(choice * _prefabs.Length)];
    }
}
