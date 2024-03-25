using UnityEngine;
using UnityEngine.UI;

public class CatHealthBar : MonoBehaviour
{
    private UIFollowTransform followTransform;
    private Slider healthSlider;
    private Cat cat;

    private void Awake()
    {
        followTransform = GetComponent<UIFollowTransform>();
        healthSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        healthSlider.value = cat.data.health;
    }

    public void Initialize(Cat cat)
    {
        followTransform.followTransform = cat.transform;
        healthSlider.maxValue = cat.data.maxHealth.value;
        healthSlider.value = cat.data.health;
        this.cat = cat;
    }
}
