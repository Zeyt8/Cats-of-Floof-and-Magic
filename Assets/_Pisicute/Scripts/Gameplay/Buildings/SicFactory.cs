using UnityEngine;

public class SicFactory : Building
{
    [SerializeField] private CatCollection sicCats;
    public Cat catInWaiting;

    public override void OnSpawn(HexCell cell)
    {
        GameManager.OnRoundEnd.AddListener(CreateCat);
    }

    public void TakeCat()
    {
        if (Location.battleMap != null)
        {
            if (Location.units[0].owner == owner)
            {
                ((Leader)Location.units[0]).AddCatToArmy(catInWaiting.data);
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
