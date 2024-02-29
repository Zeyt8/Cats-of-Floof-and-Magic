using TMPro;
using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI food;
    [SerializeField] private TextMeshProUGUI wood;
    [SerializeField] private TextMeshProUGUI stone;
    [SerializeField] private TextMeshProUGUI steel;
    [SerializeField] private TextMeshProUGUI sulfur;
    [SerializeField] private TextMeshProUGUI gems;

    public void SetResourcesUI(Resources playerResources, Resources resourceGain)
    {
        food.text = $"{playerResources.food} (+{resourceGain.food})";
        wood.text = $"{playerResources.wood} (+{resourceGain.wood})";
        stone.text = $"{playerResources.stone} (+{resourceGain.stone})";
        steel.text = $"{playerResources.steel} (+{resourceGain.steel})";
        sulfur.text = $"{playerResources.sulfur} (+{resourceGain.sulfur})";
        gems.text = $"{playerResources.gems} (+{resourceGain.gems})";
    }
}
