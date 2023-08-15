using UnityEngine;

public class SteelFoundryUI : BuildingUI
{
    public void Smelt()
    {
        if (Player.Instance.CurrentResources >= SteelFoundry.SmeltCost)
        {
            Player.Instance.CurrentResources -= SteelFoundry.SmeltCost;
            Player.Instance.CurrentResources += SteelFoundry.SmeltGain;
        }
    }
}
