using UnityEngine;

[CreateAssetMenu(fileName = "SpellGraphicsContainer", menuName = "Scriptable Objects/SpellGraphicsContainer", order = 1)]
public class SpellGraphicsContainer : ScriptableObject
{
    public UDictionary<string, GameObject> graphics = new UDictionary<string, GameObject>();

    public GameObject this[string b]
    {
        get => graphics[b];
        set => graphics[b] = value;
    }
}
