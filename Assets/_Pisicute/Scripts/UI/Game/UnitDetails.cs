using TMPro;
using UnityEngine;

public class UnitDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Activate(UnitObject unit)
    {
        gameObject.SetActive(true);
        text.text = unit.name;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
