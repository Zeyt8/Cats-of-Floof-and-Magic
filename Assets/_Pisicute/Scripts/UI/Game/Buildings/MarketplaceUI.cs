using TMPro;
using UnityEngine;

public class MarketplaceUI : BuildingUI
{
    [SerializeField] private TextMeshProUGUI soldFood;
    [SerializeField] private TextMeshProUGUI soldWood;
    [SerializeField] private TextMeshProUGUI soldStone;
    [SerializeField] private TextMeshProUGUI soldSteel;
    [SerializeField] private TextMeshProUGUI soldSulfur;
    [SerializeField] private TextMeshProUGUI soldGems;

    [SerializeField] private TextMeshProUGUI boughtFood;
    [SerializeField] private TextMeshProUGUI boughtWood;
    [SerializeField] private TextMeshProUGUI boughtStone;
    [SerializeField] private TextMeshProUGUI boughtSteel;
    [SerializeField] private TextMeshProUGUI boughtSulfur;
    [SerializeField] private TextMeshProUGUI boughtGems;

    private Resources.Resource? soldResource;
    private Resources.Resource? boughtResource;
    private Resources sold;
    private Resources bought;

    public void BuyResource()
    {
        if (PlayerObject.Instance.CurrentResources >= sold)
        {
            PlayerObject.Instance.CurrentResources -= sold;
            PlayerObject.Instance.CurrentResources += bought;
        }
    }

    public void SelectSoldResource(int resource)
    {
        soldResource = (Resources.Resource)resource;
        if (sold == null || bought == null) return;
        SetPrice();
    }

    public void SelectBoughtResource(int resource)
    {
        boughtResource = (Resources.Resource)resource;
        if (sold == null || bought == null) return;
        SetPrice();
    }

    private void SetPrice()
    {
        sold = new Resources(soldResource.Value) * Marketplace.ExchangeRate[(int)soldResource, (int)boughtResource].item1;
        bought = new Resources(boughtResource.Value) * Marketplace.ExchangeRate[(int)soldResource, (int)boughtResource].item2;
        soldFood.text = sold.food.ToString();
        soldWood.text = sold.wood.ToString();
        soldStone.text = sold.stone.ToString();
        soldSteel.text = sold.steel.ToString();
        soldSulfur.text = sold.sulfur.ToString();
        soldGems.text = sold.gems.ToString();
        boughtFood.text = bought.food.ToString();
        boughtWood.text = bought.wood.ToString();
        boughtStone.text = bought.stone.ToString();
        boughtSteel.text = bought.steel.ToString();
        boughtSulfur.text = bought.sulfur.ToString();
        boughtGems.text = bought.gems.ToString();
    }
}
