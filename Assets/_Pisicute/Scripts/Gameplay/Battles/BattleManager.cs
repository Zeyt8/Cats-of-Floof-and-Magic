using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private BattleMap battleMapPrefab;
    [SerializeField] private List<MapGeneratorSettings> biomeConfigurations = new List<MapGeneratorSettings>();

    private List<BattleMap> battleMaps = new List<BattleMap>();

    public BattleMap GenerateBattle(int terrain, List<Leader> leaders)
    {
        for (int i = 0; i < battleMaps.Count; i++)
        {
            if (battleMaps[i] == null)
            {
                return battleMaps[i] = GenerateBattleMap(terrain, (i + 1) * -250, leaders);
            }
        }
        BattleMap battleMap = GenerateBattleMap(terrain, (battleMaps.Count + 1) * -250, leaders);
        battleMaps.Add(battleMap);
        return battleMap;
    }

    private BattleMap GenerateBattleMap(int terrain, int z, List<Leader> leaders)
    {
        BattleMap bm = Instantiate(battleMapPrefab);
        bm.generator.settings = biomeConfigurations[terrain];
        bm.GenerateBattleMap(15, 10, leaders);
        bm.transform.position = new Vector3(0, 0, z);
        return bm;
    }
}
