using UnityEngine;
using System.Collections.Generic;

public class LeaderList : MonoBehaviour
{
    [SerializeField] private LeaderIcon leaderIconPrefab;
    private List<LeaderIcon> leaderIcons = new List<LeaderIcon>();

    private void OnEnable()
    {
        GameEvents.OnLeaderRecruited.AddListener(SetupLeaderIcons);
    }

    private void OnDisable()
    {
        GameEvents.OnLeaderRecruited.RemoveListener(SetupLeaderIcons);
    }

    private void SetupLeaderIcons(int team)
    {
        if (team != PlayerObject.Instance.playerNumber) return;
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Leader leader in PlayerObject.Instance.leaders)
        {
            LeaderIcon leaderIcon = Instantiate(leaderIconPrefab, transform);
            leaderIcon.Leader = leader;
            leaderIcons.Add(leaderIcon);
        }
    }
}
