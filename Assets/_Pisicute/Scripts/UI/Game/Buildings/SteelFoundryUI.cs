using UnityEngine;

public class SteelFoundryUI : BuildingUI
{
    public void Smelt()
    {
        if (PlayerObject.Instance.CurrentResources >= SteelFoundry.SmeltCost)
        {
            PlayerObject.Instance.CurrentResources -= SteelFoundry.SmeltCost;
            PlayerObject.Instance.CurrentResources += SteelFoundry.SmeltGain;
        }
    }
}
