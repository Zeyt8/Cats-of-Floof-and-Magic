using UnityEngine;
using UnityEngine.UI;

public class LeaderIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider floofSlider;
    [SerializeField] private Slider movementPointsSlider;

    public Leader Leader
    {
        get => leader;
        set
        {
            leader = value;
            icon.sprite = leader.icon;
        }
    }
    private Leader leader;

    private void Update()
    {
        floofSlider.maxValue = Leader.maxFloof;
        floofSlider.value = Leader.currentFloof;
        movementPointsSlider.maxValue = Leader.Speed;
        movementPointsSlider.value = Leader.movementPoints;
    }

    public void GoToLeaderPosition()
    {
        LevelManager.Instance.MoveCamera(new Vector2(Leader.transform.position.x, Leader.transform.position.z));
    }
}
