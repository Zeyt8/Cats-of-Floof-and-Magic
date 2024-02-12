using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    private Sprite defaultSprite;

    private void Awake()
    {
        defaultSprite = icon.sprite;
    }

    public void SetIcon(Sprite icon, string text)
    {
        if (icon == null)
        {
            this.icon.sprite = defaultSprite;
            this.text.text = "";
            return;
        }
        this.icon.sprite = icon;
        this.text.text = $"<b>{text}</b>";
    }
}
