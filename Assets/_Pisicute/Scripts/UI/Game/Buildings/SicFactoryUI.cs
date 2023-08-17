using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SicFactoryUI : BuildingUI
{
    [SerializeField] private Image catImage;
    [SerializeField] private TextMeshProUGUI catName;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        Cat cat = ((SicFactory)currentBuilding).catInWaiting;
        if (cat)
        {
            catImage.sprite = cat.icon;
            catName.text = cat.data.type.ToString();
        }
        else
        {
            catImage.sprite = null;
            catName.text = "";
        }
    }

    public void TakeCat()
    {
        ((SicFactory)currentBuilding).TakeCat();
    }
}
