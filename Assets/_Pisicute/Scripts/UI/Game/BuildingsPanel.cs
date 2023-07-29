using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsPanel : MonoBehaviour
{
    [SerializeField]
    private BuildingCollection buildingCollection;
    [SerializeField]
    private Image buildingIconPrefab;
    [SerializeField]
    private Transform buildingIconsParent;

    private List<Image> buildingIcons = new List<Image>();

    private void Start()
    {
        foreach (KeyValuePair<BuildingTypes, Building> building in buildingCollection.buildings)
        {
            Image buildingIcon = Instantiate(buildingIconPrefab, buildingIconsParent);
            buildingIcon.sprite = building.Value.icon;
            buildingIcon.GetComponent<Button>().onClick.AddListener(() => SelectBuilding(buildingIcon, building.Key));
        }
    }

    private void SelectBuilding(Image image, BuildingTypes buildingType)
    {
        foreach (Image buildingIcon in buildingIcons)
        {
            DeselectBuilding(buildingIcon);
        }
        image.color = new Color(1, 1, 1, 1);
        Player.Instance.buildingToBuild = buildingType;
    }

    private void DeselectBuilding(Image buildingIcon)
    {
        buildingIcon.color = new Color(1, 1, 1, 0.5f);
    }
}
