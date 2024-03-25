using System;
using UnityEngine;

public class SplitAbility : CatAbility
{
    [SerializeField] private Cat newCat;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return cell => cat.IsValidDestination(cell) && cell.coordinates.DistanceTo(cat.Location.coordinates) < 3;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return cell =>
        {
        };
    }

    public override PlayerObject.Action<HexCell> AfterCasting(Cat caster)
    {
        return (cell) =>
        {
            SpawnSplitCat(caster, cell, 3);
        };
    }

    private void SpawnSplitCat(Cat caster, HexCell cell, int index)
    {
        caster.battleMap.InstantiateCat(newCat, caster.owner, cell);
        index -= 1;
        if (index > 0)
        {
            PlayerObject.Instance.InitiateSelectCellForEffect(
                GetAvailableTargets(caster),
                (cell) => { },
                cell =>
                {
                    SpawnSplitCat(caster, cell, index);
                });
        }
        else
        {
            EndTurn(caster);
            Destroy(caster.gameObject);
        }
    }
}
