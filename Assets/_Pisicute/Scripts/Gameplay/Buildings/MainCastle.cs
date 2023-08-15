using UnityEngine;

public class MainCastle : Building
{
    [SerializeField] Leader leaderPrefab;

    public void CreateLeader(HexCell cell)
    {
        cell.AddUnit(Instantiate(leaderPrefab), 0);
    }
}
