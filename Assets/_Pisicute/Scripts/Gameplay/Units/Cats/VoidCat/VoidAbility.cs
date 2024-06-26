using JSAM;
using System;
using UnityEngine;

public class VoidAbility : CatAbility
{
    [SerializeField] private int floofRestored;
    [SerializeField] private GameObject voidGraphics;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => cell == cat.Location;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        Instantiate(voidGraphics, caster.transform.position, Quaternion.identity);
        caster.leader.GainFloof(floofRestored);
        EndTurn(caster);
        AudioManager.PlaySound(AudioLibrarySounds.Void);
        return (cell) => { };
    }
}
