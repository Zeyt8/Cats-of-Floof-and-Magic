using UnityEngine;

[CreateAssetMenu(fileName = "BuildingCollection", menuName = "Scriptable Objects/BuildingCollection", order = 1)]
public class BuildingCollection : ScriptableObject
{
    public UDictionary<BuildingTypes, Building> buildings = new UDictionary<BuildingTypes, Building>();

    public Building this[BuildingTypes b]
    {
        get => buildings[b];
        set => buildings[b] = value;
    }
}