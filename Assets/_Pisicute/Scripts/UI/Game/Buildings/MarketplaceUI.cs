using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class MarketplaceUI : BuildingUI
{
    [SerializeField] private List<TextMeshProUGUI> soldResources;
    [SerializeField] private List<TextMeshProUGUI> boughtResources;

    private Resources.Resource? soldResource;
    private Resources.Resource? boughtResource;
    private Resources sold;
    private Resources bought;

    public void BuyResource()
    {
        if (soldResource == null || boughtResource == null || soldResource == boughtResource) return;
        if (PlayerObject.Instance.CurrentResources >= sold)
        {
            PlayerObject.Instance.CurrentResources -= sold;
            PlayerObject.Instance.CurrentResources += bought;
        }
    }

    public void SelectSoldResource(int resource)
    {
        soldResource = (Resources.Resource)resource;
        if (soldResource == null || boughtResource == null) return;
        SetPrice();
    }

    public void SelectBoughtResource(int resource)
    {
        boughtResource = (Resources.Resource)resource;
        if (soldResource == null || boughtResource == null) return;
        SetPrice();
    }

    private void SetPrice()
    {
        Pair<int, int> rate = Marketplace.ExchangeRate[(int)soldResource, (int)boughtResource];
        for (int i = 0; i < soldResources.Count; i++)
        {
            soldResources[i].text = "";
            boughtResources[i].text = "";
        }
        if (rate != null)
        {
            sold = new Resources(soldResource.Value) * Marketplace.ExchangeRate[(int)soldResource, (int)boughtResource].item1;
            bought = new Resources(boughtResource.Value) * Marketplace.ExchangeRate[(int)soldResource, (int)boughtResource].item2;
            soldResources[(int)soldResource].text = sold[(int)soldResource].ToString();
            boughtResources[(int)boughtResource].text = bought[(int)boughtResource].ToString();
        }
    }
}
