using TMPro;
using UnityEngine;

public class BuildingDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Activate(Building building)
    {
        gameObject.SetActive(true);
        text.text = building.description;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
