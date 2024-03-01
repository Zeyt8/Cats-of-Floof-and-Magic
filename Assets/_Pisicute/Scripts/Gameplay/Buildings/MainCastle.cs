using System.Collections;
using UnityEngine;

public class MainCastle : Building
{
    [SerializeField] Leader leaderPrefab;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        if (owner == PlayerObject.Instance.playerNumber)
        {
            LevelManager.Instance.MoveCamera(new Vector2(transform.position.x, transform.position.z));
        }
    }

    public void CreateLeader(HexCell cell)
    {
        Leader leader = Instantiate(leaderPrefab);
        leader.ChangeOwner(owner);
        cell.AddUnit(leader, 0);
    }
}
