using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private BattleMap battleMapPrefab;
    [SerializeField] private List<MapGeneratorSettings> biomeConfigurations = new List<MapGeneratorSettings>();

    private List<BattleMap> battleMaps = new List<BattleMap>();

    public void GenerateBattle(int terrain)
    {
        for (int i = 0; i < battleMaps.Count; i++)
        {
            if (battleMaps[i] == null)
            {
                battleMaps[i] = GenerateBattleMap(terrain, (i + 1) * -100);
                return;
            }
        }
        battleMaps.Add(GenerateBattleMap(terrain, (battleMaps.Count + 1) * -100));
    }

    private BattleMap GenerateBattleMap(int terrain, int y)
    {
        BattleMap bm = Instantiate(battleMapPrefab);
        bm.generator.settings = biomeConfigurations[terrain];
        bm.GenerateBattleMap();
        bm.transform.position = new Vector3(0, y, 0);
        return bm;
    }
}
