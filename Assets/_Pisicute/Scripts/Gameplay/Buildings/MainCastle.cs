using JSAM;
using System.Collections;
using UnityEngine;

public class MainCastle : Building
{
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
        if (PlayerObject.Instance.CurrentResources >= new Resources(10, 0, 0, 0, 0, 0) && !cell.Unit)
        {
            PlayerObject.Instance.CurrentResources -= new Resources(10, 0, 0, 0, 0, 0);
            PlayerObject.Instance.SpawnLeaderServerRpc(cell.coordinates, owner);
        }
    }

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.Castle);
    }
}
