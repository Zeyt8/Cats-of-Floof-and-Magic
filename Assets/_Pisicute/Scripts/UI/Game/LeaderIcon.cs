using UnityEngine;
using UnityEngine.UI;

public class LeaderIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider floofSlider;
    [SerializeField] private Slider movementPointsSlider;

    public Leader leader;

    private void Update()
    {
        floofSlider.maxValue = leader.maxFloof;
        floofSlider.value = leader.currentFloof;
        movementPointsSlider.maxValue = leader.Speed;
        movementPointsSlider.value = leader.movementPoints;
    }

    public void GoToLeaderPosition()
    {
        LevelManager.Instance.MoveCamera(new Vector2(leader.transform.position.x, leader.transform.position.z));
    }
}
