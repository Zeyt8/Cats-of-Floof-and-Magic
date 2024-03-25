using System;
using UnityEngine;

public class VoidAbility : CatAbility
{
    [SerializeField] private int floofRestored;
    [SerializeField] private GameObject voidGraphics;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => false;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        caster.leader.GainFloof(floofRestored);
        EndTurn(caster);
        Instantiate(voidGraphics, caster.transform.position, Quaternion.identity);
        return (cell) => { };
    }
}
