using UnityEngine;
using System;

public abstract class CatAbility : MonoBehaviour
{
    public Sprite icon;
    public int cooldown;
    [TextArea]
    public string title;
    [TextArea]
    public string description;

    public enum ActivationType
    {
        Instant,
        RequiresTarget
    }
    public ActivationType activationType;

    public abstract Func<HexCell, bool> GetAvailableTargets(Cat cat);

    public virtual PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        EndTurn(caster);
        return (cell) => { };
    }

    protected void EndTurn(Cat caster)
    {
        PlayerObject.Instance.EndTurnOnBattleMapServerRpc(BattleManager.GetBattleMapIndex(caster.battleMap));
    }

    public static Quaternion AbilityRotation(UnitObject caster, UnitObject target)
    {
        return Quaternion.LookRotation(target.transform.position - caster.transform.position, Vector3.up);
    }
}
