using UnityEngine;

public class SicFactoryUI : BuildingUI
{
    [SerializeField] private CatIcon catImage;
    [SerializeField] private Sprite emptySprite;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        Cat cat = ((SicFactory)currentBuilding).catInWaiting;
        if (cat)
        {
            catImage.SetIcon(cat.icon, cat.data.type.GetPrettyName());
        }
        else
        {
            catImage.SetIcon(emptySprite, "");
        }
    }

    public void TakeCat()
    {
        ((SicFactory)currentBuilding).TakeCat();
        Initialize(currentBuilding);
    }
}
