using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGroundsUI : BuildingUI
{
    [SerializeField] private ClickableCatIcon catIconPrefab;
    [SerializeField] private CatCollection allCats;
    [SerializeField] private CatCollection rareCats;
    [SerializeField] private CatCollection epicCats;
    [SerializeField] private CatCollection legendaryCats;
    [SerializeField] private Transform units1;
    [SerializeField] private Transform units2;

    private SingleSelectGroup singleSelectGroup1;
    private SingleSelectGroup singleSelectGroup2;

    private Cat cat1;
    private Cat cat2;

    private void Awake()
    {
        singleSelectGroup1 = units1.GetComponent<SingleSelectGroup>();
        singleSelectGroup2 = units2.GetComponent<SingleSelectGroup>();
    }

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        if (building.Location.units.Count > 0)
        {
            SetCats();
        }
    }

    public void Upgrade()
    {
        int rarity = 0;
        // get biggest rarity
        if (legendaryCats.cats.ContainsKey(cat1.data.type) || legendaryCats.cats.ContainsKey(cat2.data.type))
        {
            rarity = 3;
        }
        else if (epicCats.cats.ContainsKey(cat1.data.type) || epicCats.cats.ContainsKey(cat2.data.type))
        {
            rarity = 2;
        }
        else if (rareCats.cats.ContainsKey(cat1.data.type) || rareCats.cats.ContainsKey(cat2.data.type))
        {
            rarity = 1;
        }
        // remove cats
        ((Leader)currentBuilding.Location.Unit).army.Remove(cat1.data);
        ((Leader)currentBuilding.Location.Unit).army.Remove(cat2.data);
        // add new cat
        CatData catToAdd = new CatData();
        switch (rarity)
        {
            case 0:
                catToAdd = rareCats.cats.Values.GetRandom().data;
                break;
            case 1:
                catToAdd = epicCats.cats.Values.GetRandom().data;
                break;
            case 2:
                catToAdd = legendaryCats.cats.Values.GetRandom().data;
                break;
            case 3:
                catToAdd = legendaryCats.cats.Values.GetRandom().data;
                break;
            default:
                break;
        }
        PlayerObject.Instance.AddCatDataToLeaderServerRpc(catToAdd, currentBuilding.Location.coordinates, currentBuilding.owner);
        SetCats();
    }

    private void SetCats()
    {
        foreach (var unit in ((Leader)currentBuilding.Location.Unit).army)
        {
            ClickableCatIcon catIcon = Instantiate(catIconPrefab, units1);
            catIcon.SetCat(allCats[unit.type]);
            singleSelectGroup1.images.Add(catIcon.icon);
            catIcon.onClick += ((cat) =>
            {
                singleSelectGroup1.Select(catIcon.gameObject);
                cat1 = cat;
            });

            ClickableCatIcon catIcon2 = Instantiate(catIconPrefab, units2);
            catIcon2.SetCat(allCats[unit.type]);
            singleSelectGroup2.images.Add(catIcon2.icon);
            catIcon2.onClick += ((cat) =>
            {
                singleSelectGroup2.Select(catIcon2.gameObject);
                cat2 = cat;
            });
        }
    }
}
