using TMPro;
using UnityEngine;

public class UnitDetails : MonoBehaviour
{
    [SerializeField] private GameObject unitDetails;
    [SerializeField] private GameObject battleDetails;
    [SerializeField] private TextMeshProUGUI text;

    private HexCell currentCell;

    public void Activate(HexCell cell, UnitObject unit)
    {
        currentCell = cell;
        gameObject.SetActive(true);
        if (cell.units.Count == 1)
        {
            unitDetails.SetActive(true);
            battleDetails.SetActive(false);
            text.text = unit.name;
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
