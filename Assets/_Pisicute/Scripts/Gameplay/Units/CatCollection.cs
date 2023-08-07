using UnityEngine;

[CreateAssetMenu(fileName = "UnitCollection", menuName = "Scriptable Objects/UnitCollection", order = 1)]
public class CatCollection : ScriptableObject
{
    public UDictionary<CatTypes, Cat> cats = new UDictionary<CatTypes, Cat>();
    
    public Cat this[CatTypes b]
    {
        get => cats[b];
        set => cats[b] = value;
    }
}
