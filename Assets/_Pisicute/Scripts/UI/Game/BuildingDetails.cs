using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject button;
    [SerializeField] private TextMeshProUGUI buttonText;
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
        text.text = $"<b>{building.title}</b>\n{building.description}";
        icon.sprite = building.icon;
        if (building.HasUIPanel && building.owner == PlayerObject.Instance.playerNumber)
        {
            button.SetActive(true);
            buttonText.text = "Building Menu";
        }
        else if (building.HasAction && building.owner == PlayerObject.Instance.playerNumber)
        {
            button.SetActive(true);
            buttonText.text = "Use";
        }
        else
        {
            button.SetActive(false);
        }
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
        if (currentBuilding.HasUIPanel)
        {
            currentOpenUIPanel = currentBuilding.OpenUIPanel();
        }
        else
        {
            currentBuilding.action?.Invoke();
        }
    }

    private void DeactivateBuildingUI()
    {
        if (currentOpenUIPanel)
        {
            Destroy(currentOpenUIPanel.gameObject);
        }
    }
}
