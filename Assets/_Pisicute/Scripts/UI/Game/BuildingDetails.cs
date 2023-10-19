using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject button;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    private Building currentBuilding;
    private BuildingUI currentOpenUIPanel;

    private void OnEnable()
    {
        playerInputHandler.OnCancel.AddListener(DeactivateBuildingUI);
    }

    private void OnDisable()
    {
        playerInputHandler.OnCancel.RemoveListener(DeactivateBuildingUI);
    }

    public void Activate(Building building)
    {
        currentBuilding = building;
        gameObject.SetActive(true);
        text.text = $"<b>{building.type}</b>\n{building.description}";
        icon.sprite = building.icon;
        button.SetActive(building.HasUIPanel);
        DeactivateBuildingUI();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        DeactivateBuildingUI();
    }

    public void ActivateBuildingUI()
    {
        DeactivateBuildingUI();
        currentOpenUIPanel = currentBuilding.OpenUIPanel();
    }

    private void DeactivateBuildingUI()
    {
        if (currentOpenUIPanel)
        {
            Destroy(currentOpenUIPanel.gameObject);
        }
    }
}
