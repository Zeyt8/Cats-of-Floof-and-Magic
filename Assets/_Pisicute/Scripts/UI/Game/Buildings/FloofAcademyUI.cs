using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloofAcademyUI : BuildingUI
{
    [SerializeField] private CatIcon catIcon;
    [SerializeField] private List<TextMeshProUGUI> resources = new List<TextMeshProUGUI>();
    [SerializeField] private Sprite emptySprite;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        Cat cat = ((FloofAcademy)currentBuilding).catInWaiting;
        if (cat)
        {
            catIcon.SetIcon(cat.icon, cat.data.type.GetPrettyName());
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].text = ((FloofAcademy)currentBuilding).catInWaiting.sellCost[i].ToString();
            }
        }
        else
        {
            catIcon.SetIcon(emptySprite, "");
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].text = "";
            }
        }
    }

    public void TakeCat()
    {
        ((FloofAcademy)currentBuilding).TakeCat();
        Initialize(currentBuilding);
    }
}
