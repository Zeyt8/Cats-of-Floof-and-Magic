using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] private Sprite inactive;
    [SerializeField] private Sprite active;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SwitchSprites(bool active)
    {
        if (active)
        {
            image.sprite = this.active;
        }
        else
        {
            image.sprite = inactive;
        }
    }

    public void EndTurn()
    {
        LevelManager.Instance.EndTurn();
    }
}
