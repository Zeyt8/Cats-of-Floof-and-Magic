using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private BattleMap battleMapPrefab;

    public void GenerateBattle(int terrain)
    {
        BattleMap bm = Instantiate(battleMapPrefab);
        bm.transform.position = new Vector3(0, -10, 0);
        bm.GenerateBattleMap();
    }
}
