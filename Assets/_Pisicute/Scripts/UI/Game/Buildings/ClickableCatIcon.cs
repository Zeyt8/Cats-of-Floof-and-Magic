using System;
using UnityEngine;
using UnityEngine.UI;

public class ClickableCatIcon : MonoBehaviour
{
    public Image icon;
    private Cat cat;
    public Action<Cat> onClick;

    public void SetCat(Cat cat)
    {
        this.cat = cat;
        icon.sprite = cat.icon;
    }

    public void OnClick()
    {
        onClick?.Invoke(cat);
    }
}
