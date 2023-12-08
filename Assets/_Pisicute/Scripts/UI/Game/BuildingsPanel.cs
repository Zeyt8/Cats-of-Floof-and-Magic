using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsPanel : MonoBehaviour
{
    [SerializeField]
    private BuildingCollection buildingCollection;
    [SerializeField]
    private BuildingIcon buildingIconPrefab;
    [SerializeField]
    private Transform buildingIconsParent;

    private List<BuildingIcon> buildingIcons = new List<BuildingIcon>();

    private void Start()
    {
        foreach (KeyValuePair<BuildingTypes, Building> building in buildingCollection.buildings)
        {
            BuildingIcon buildingIcon = Instantiate(buildingIconPrefab, buildingIconsParent);
            buildingIcon.image.sprite = building.Value.icon;
            buildingIcon.GetComponent<Button>().onClick.AddListener(() => SelectBuilding(buildingIcon, building.Key));
            buildingIcon.SetText(building.Value.title, building.Value.description);
        }
    }

    private void SelectBuilding(BuildingIcon buildingIcon, BuildingTypes buildingType)
    {
        foreach (BuildingIcon bi in buildingIcons)
        {
            bi.Deselect();
        }
        buildingIcon.Select();
        PlayerObject.Instance.buildingToBuild = buildingType;
    }
}
