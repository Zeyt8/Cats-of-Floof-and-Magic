using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    protected Building currentBuilding;

    public virtual void Initialize(Building building)
    {
        currentBuilding = building;
    }
}
