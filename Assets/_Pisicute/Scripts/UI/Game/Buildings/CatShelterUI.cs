using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatShelterUI : BuildingUI
{
    [SerializeField] private ClickableCatIcon catIconPrefab;
    [SerializeField] private CatCollection allCats;
    [SerializeField] private Transform catsTransform;
    [SerializeField] private TextMeshProUGUI wood;
    [SerializeField] private TextMeshProUGUI stone;
    [SerializeField] private TextMeshProUGUI steel;
    [SerializeField] private TextMeshProUGUI sulfur;
    [SerializeField] private TextMeshProUGUI gems;

    private Cat currentCat;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        SetCats(((Leader)building.Location.units[0]).army);
    }

    public void SellCat()
    {
        if (((Leader)currentBuilding.Location.units[0]).army.Remove(currentCat.data))
        {
            PlayerObject.Instance.CurrentResources += currentCat.sellCost;
            SetCats(((Leader)currentBuilding.Location.units[0]).army);
            currentCat = null;
        }
    }

    private void SetResources(Resources sellCost)
    {
        wood.text = sellCost.wood.ToString();
        stone.text = sellCost.stone.ToString();
        steel.text = sellCost.steel.ToString();
        sulfur.text = sellCost.sulfur.ToString();
        gems.text = sellCost.gems.ToString();
    }

    private void SetCats(List<CatData> cats)
    {
        foreach (Transform transform in catsTransform)
        {
            Destroy(transform.gameObject);
        }
        foreach (CatData cat in cats)
        {
            ClickableCatIcon icon = Instantiate(catIconPrefab, catsTransform);
            icon.SetCat(allCats[cat.type]);
            icon.onClick += OnCatClick;
        }
    }

    private void OnCatClick(Cat cat)
    {
        SetResources(cat.sellCost);
        currentCat = cat;
    }
}
