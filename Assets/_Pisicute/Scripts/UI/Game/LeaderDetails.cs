using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderDetails : MonoBehaviour
{
    [SerializeField] private GameObject unitDetails;
    [SerializeField] private GameObject battleDetails;
    [Header("Unit Details")]
    [SerializeField] private Image unitIcon;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Slider floofSlider;
    [SerializeField] private Slider movementPointsSlider;

    private HexCell currentCell;

    public void Activate(HexCell cell, Leader unit)
    {
        currentCell = cell;
        gameObject.SetActive(true);
        if (cell.units.Count == 1)
        {
            unitDetails.SetActive(true);
            battleDetails.SetActive(false);
            unitIcon.sprite = unit.icon;
            description.text = $"Player {unit.owner}";
            floofSlider.maxValue = unit.maxFloof;
            floofSlider.value = unit.currentFloof;
            movementPointsSlider.maxValue = unit.Speed;
            movementPointsSlider.value = unit.movementPoints;
        }
        else if (cell.units.Count > 1)
        {
            unitDetails.SetActive(false);
            battleDetails.SetActive(true);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void GoToBattle()
    {
        Player.Instance.GoToBattleMap(currentCell.battleMap);
    }
}
