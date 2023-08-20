using UnityEngine;

public class MainCastle : Building
{
    [SerializeField] Leader leaderPrefab;

    public void CreateLeader(HexCell cell)
    {
        Leader leader = Instantiate(leaderPrefab);
        leader.ChangeOwner(owner);
        cell.AddUnit(leader, 0);
    }
}
