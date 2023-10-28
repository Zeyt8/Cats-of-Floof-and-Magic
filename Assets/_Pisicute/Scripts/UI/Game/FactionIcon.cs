using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private UDictionary<Factions, Sprite> sprites;

    public void Initialize(Factions faction, int level, int threshold)
    {
        icon.sprite = sprites[faction];
        text.text = $"{level}/{threshold}";
    }
}
