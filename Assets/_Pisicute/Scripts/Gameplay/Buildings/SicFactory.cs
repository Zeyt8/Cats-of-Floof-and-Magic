using UnityEngine;

public class SicFactory : Building
{
    [SerializeField] private CatCollection sicCats;
    public Cat catInWaiting;

    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        GameEvents.OnRoundEnd.AddListener(CreateCat);
    }

    public void TakeCat()
    {
        if (Location.battleMap == null && catInWaiting != null)
        {
            if (Location.Unit.owner == owner)
            {
                ((Leader)Location.Unit).AddCatToArmy(catInWaiting.data);
                catInWaiting = null;
            }
        }
    }

    private void CreateCat()
    {
        if (catInWaiting == null && owner == PlayerObject.Instance.playerNumber)
        {
            catInWaiting = sicCats.cats.Values.GetRandom();
        }
    }
}
