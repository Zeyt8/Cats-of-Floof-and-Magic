using Unity.Collections;
using UnityEngine;

public class OrientalCat : Cat
{
    [SerializeField] private CatAbility wildAbility;

    public override void OnEncounterStart()
    {
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new OrientalPassive(-1), BattleManager.GetBattleMapIndex(battleMap), Location.coordinates, owner);
        base.OnEncounterStart();
    }

    public override void AddStatusEffect(StatusEffect effect)
    {
        base.AddStatusEffect(effect);
        if (effect is WildFactionStatusEffect)
        {
            abilities.Add(wildAbility);
        }
    }

    public override void RemoveStatusEffect(FixedString32Bytes type)
    {
        base.RemoveStatusEffect(type);
        if (type.Equals(typeof(WildEffect).ToString()))
        {
            abilities.Remove(wildAbility);
        }
    }
}
