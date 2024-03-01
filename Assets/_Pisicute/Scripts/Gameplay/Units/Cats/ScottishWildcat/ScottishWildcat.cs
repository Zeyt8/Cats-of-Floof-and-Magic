using Unity.Collections;
using UnityEngine;

public class ScottishWildcat : Cat
{
    [SerializeField] private CatAbility wildAbility;

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
