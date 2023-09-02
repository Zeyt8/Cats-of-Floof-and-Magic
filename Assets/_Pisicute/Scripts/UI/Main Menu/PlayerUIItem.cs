using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image icon;

    public void Set(string name, Color color)
    {
        playerName.text = name;
        icon.color = color;
    }
}
