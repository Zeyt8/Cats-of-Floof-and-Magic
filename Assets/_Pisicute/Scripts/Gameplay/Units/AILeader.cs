using System.Collections.Generic;
using UnityEngine;

public class AILeader : Leader
{
    [SerializeField] List<CatTypes> possibleCats = new List<CatTypes>();

    protected override void Start()
    {
        foreach (CatTypes catType in possibleCats)
        {
            AddCatToArmy(allCats[catType].data);
        }
    }
}
