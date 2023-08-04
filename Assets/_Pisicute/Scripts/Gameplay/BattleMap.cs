using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    [SerializeField] private HexMapGenerator generator;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 5;

    [ContextMenu("Generate Battle Map")]
    public void GenerateBattleMap()
    {
        generator.GenerateMap(width, height);
    }
}
