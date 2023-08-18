using UnityEngine;

public class SicFactory : Building
{
    [SerializeField] private CatCollection sicCats;
    public Cat catInWaiting;

    public override void OnSpawn(HexCell cell)
    {
        GameEvents.OnRoundEnd.AddListener(CreateCat);
    }

    public void TakeCat()
    {
        if (Location.battleMap == null && catInWaiting != null)
        {
            if (Location.units[0].owner == owner)
            {
                ((Leader)Location.units[0]).AddCatToArmy(catInWaiting.data);
                catInWaiting = null;
            }
        }
    }

    private void CreateCat()
    {
        if (catInWaiting == null)
        {
            catInWaiting = sicCats.cats.Values.GetRandom();
        }
    }
}
