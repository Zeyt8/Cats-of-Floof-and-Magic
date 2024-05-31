using UnityEngine;

public class CalicoSplitCat : Cat
{
    [SerializeField] private Material[] _monoMaterials = new Material[3];

    public void SetColor(int index)
    {
        Material[] tempMaterials = Mesh.materials;
        tempMaterials[0] = _monoMaterials[index];
        tempMaterials[3] = _monoMaterials[index];
        Mesh.materials = tempMaterials;
    }
}
