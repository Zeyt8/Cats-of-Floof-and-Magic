using TMPro;
using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wood;
    [SerializeField] private TextMeshProUGUI stone;
    [SerializeField] private TextMeshProUGUI steel;
    [SerializeField] private TextMeshProUGUI sulfur;
    [SerializeField] private TextMeshProUGUI gems;

    public void SetResourcesUI(Resources playerResources)
    {
        wood.text = playerResources.wood.ToString();
        stone.text = playerResources.stone.ToString();
        steel.text = playerResources.steel.ToString();
        sulfur.text = playerResources.sulfur.ToString();
        gems.text = playerResources.gems.ToString();
    }
}
