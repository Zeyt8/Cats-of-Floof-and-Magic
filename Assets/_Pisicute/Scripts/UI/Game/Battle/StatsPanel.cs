using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI statsText;

    public void SetStats(CatData data)
    {
        healthSlider.maxValue = data.maxHealth.value;
        healthSlider.value = data.health;
        statsText.text = $"Health: {data.health} / Power: {data.power.value} / Speed: {data.speed.value}";
    }

    public void Empty()
    {
        healthSlider.maxValue = 0;
        healthSlider.value = 0;
        statsText.text = "";
    }
}
