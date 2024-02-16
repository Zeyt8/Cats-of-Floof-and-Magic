using UnityEngine;

public class FloofAcademy : Building
{
    [SerializeField] private CatCollection magicalCats;
    public Cat catInWaiting;

    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(CreateCat);
    }

    public void TakeCat()
    {
        if (Location.battleMap == null && catInWaiting != null && PlayerObject.Instance.CurrentResources >= catInWaiting.sellCost)
        {
            if (Location.Unit.owner == owner)
            {
                PlayerObject.Instance.AddCatDataToLeaderServerRpc(catInWaiting.data, Location.coordinates, owner);
                PlayerObject.Instance.CurrentResources -= catInWaiting.sellCost;
                catInWaiting = null;
            }
        }
    }

    [ContextMenu("Create Cat")]
    private void CreateCat()
    {
        if (catInWaiting == null && owner == PlayerObject.Instance.playerNumber)
        {
            catInWaiting = magicalCats.cats.Values.GetRandom();
        }
    }
}
