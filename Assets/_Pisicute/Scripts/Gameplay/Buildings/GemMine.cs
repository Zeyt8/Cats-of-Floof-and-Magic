using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMine : Building
{
    private void OnEnable()
    {
        GameManager.Instance.OnTurnEnd.AddListener(GenerateWood);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTurnEnd.RemoveListener(GenerateWood);
    }

    private void GenerateWood(int player)
    {
        if (player == owner && owner == Player.Instance.playerNumber)
        {
            Player.Instance.CurrentResources += new Resources(0, 0, 0, 0, 1);
        }
    }
}
