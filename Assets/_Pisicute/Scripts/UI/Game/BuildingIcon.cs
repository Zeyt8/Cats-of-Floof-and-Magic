using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingIcon : MonoBehaviour
{
    public Image image;
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string title, string description, Resources cost)
    {
        text.text = $"<b>{title}</b>\n<i>{cost}</i>\n{description}";
    }

    public void Select()
    {
        image.color = new Color(1, 1, 1, 1);
    }

    public void Deselect()
    {
        image.color = new Color(1, 1, 1, 0.5f);
    }
}
