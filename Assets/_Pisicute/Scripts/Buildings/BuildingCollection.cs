using UnityEngine;

[CreateAssetMenu(fileName = "BuildingCollection", menuName = "Scriptable Objects/BuildingCollection", order = 1)]
public class BuildingCollection : ScriptableObject
{
    public UDictionary<Buildings, Building> buildings = new UDictionary<Buildings, Building>();
}