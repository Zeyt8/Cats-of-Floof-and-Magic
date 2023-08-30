using System;
using UnityEngine;
using TMPro;

public class BuildingsDropdown : MonoBehaviour
{
    [SerializeField] private BuildingCollection allBuildings;
    private TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        for (int i = 1; i < Enum.GetValues(typeof(BuildingTypes)).Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(((BuildingTypes)i).ToString()));
        }
    }
}
