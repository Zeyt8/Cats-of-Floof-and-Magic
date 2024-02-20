using UnityEngine;

public class ResourcePickUp : Building
{
    [SerializeField] private Resources resources;

    public override void OnUnitEnter(UnitObject unit)
    {
        base.OnUnitEnter(unit);
        if (unit.owner == PlayerObject.Instance.playerNumber)
        {
            PlayerObject.Instance.CurrentResources += resources;
        }
        chunk.RemoveBuilding(this);
    }
}
