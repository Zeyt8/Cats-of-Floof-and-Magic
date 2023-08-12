using UnityEngine;

public class SteelFoundryUI : MonoBehaviour
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
