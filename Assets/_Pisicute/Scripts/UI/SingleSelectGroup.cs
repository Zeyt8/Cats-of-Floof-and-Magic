using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSelectGroup : MonoBehaviour
{
    public List<Image> images = new List<Image>();

    private void Start()
    {
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.6f);
        }
    }

    public void Select(GameObject gameObject)
    {
        foreach (Image image in images)
        {
            if (image.gameObject == gameObject)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }
            else
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.6f);
            }
        }
    }
}
