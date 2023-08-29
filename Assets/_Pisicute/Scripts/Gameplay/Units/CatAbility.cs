using UnityEngine;
using System;

public abstract class CatAbility : MonoBehaviour
{
    public Sprite icon;
    public int cooldown;

    public abstract Func<HexCell, bool> GetAvailableTargets(Cat cat);

    public virtual Player.Action<HexCell> CastAbility(Cat caster)
    {
        caster.OnAbilityCasted(this);
        return (cell) => { };
    }
}
