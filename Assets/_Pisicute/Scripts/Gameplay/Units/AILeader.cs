using System.Collections.Generic;
using UnityEngine;

public class AILeader : Leader
{
    [SerializeField] private int numberOfCats;
    [SerializeField] private List<CatTypes> possibleCats = new List<CatTypes>();

    protected override void Start()
    {
        for (int i = 0; i < numberOfCats; i++)
        {
            AddCatToArmy(allCats[possibleCats.GetRandom()].data);
        }
    }
}
