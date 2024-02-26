using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private BattleMap battleMapPrefab;
    [SerializeField] private List<MapGeneratorSettings> biomeConfigurations = new List<MapGeneratorSettings>();

    private static List<BattleMap> battleMaps = new List<BattleMap>();

    public static BattleMap GetBattleMap(int i)
    {
        return battleMaps[i];
    }

    public static int GetBattleMapIndex(BattleMap battleMap)
    {
        return battleMaps.IndexOf(battleMap);
    }

    public BattleMap GenerateBattle(int terrain, List<Leader> leaders, int seed)
    {
        print(seed);
        for (int i = 0; i < battleMaps.Count; i++)
        {
            if (battleMaps[i] == null)
            {
                return battleMaps[i] = GenerateBattleMap(terrain, (i + 1) * -250, leaders, seed);
            }
        }
        BattleMap battleMap = GenerateBattleMap(terrain, (battleMaps.Count + 1) * -250, leaders, seed);
        battleMaps.Add(battleMap);
        return battleMap;
    }

    private BattleMap GenerateBattleMap(int terrain, int z, List<Leader> leaders, int seed)
    {
        BattleMap bm = Instantiate(battleMapPrefab);
        biomeConfigurations[terrain].seed = seed;
        bm.generator.settings = biomeConfigurations[terrain];
        bm.GenerateBattleMap(15, 10, leaders);
        bm.transform.position = new Vector3(0, 0, z);
        return bm;
    }

    public static void RemoveBattle(BattleMap battleMap)
    {
        battleMaps.Remove(battleMap);
        Destroy(battleMap.gameObject);
    }

    public static void EndWorldTurn()
    {
        for (int i = 0; i < battleMaps.Count; i++)
        {
            battleMaps[i].OnWorldTurnEnd();
        }
    }
}
