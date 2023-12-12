using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;

    public void SetIcon(Sprite icon, string text)
    {
        this.icon.sprite = icon;
        this.text.text = $"<b>{text}</b>";
    }
}
