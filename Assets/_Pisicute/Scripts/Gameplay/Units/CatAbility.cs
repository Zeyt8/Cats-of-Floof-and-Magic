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
        PlayerObject.Instance.EndTurnOnBattleMapServerRpc(BattleManager.GetBattleMapIndex(caster.battleMap));
        return (cell) => { };
    }
}
