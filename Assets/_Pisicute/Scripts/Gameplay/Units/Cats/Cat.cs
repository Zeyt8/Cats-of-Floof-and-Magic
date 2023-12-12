using System;
using System.Collections.Generic;
using UnityEngine;

public class Cat : UnitObject
{
    public CatData data;
    public Resources sellCost;
    public override int Speed => data.speed.value;
    public List<CatAbility> abilities;
    [HideInInspector] public Leader leader;
    [HideInInspector] public BattleMap battleMap;

    protected override void Start()
    {
        base.Start();
        CalculateStats();
        SetTurnActive(false);
    }

    public void CalculateStats()
    {
        data.maxHealth.value = data.maxHealth.baseValue;
        data.power.value = data.power.baseValue;
        data.speed.value = data.speed.baseValue;
        foreach (StatusEffect status in statusEffects)
        {
            status.StatModifier(ref data);
        }
    }

    public override void DealDamage(UnitObject target, int damage)
    {
        base.DealDamage(target, damage);
        target.TakeDamage(this, damage);
    }

    public override void TakeDamage(UnitObject attacker, int damage)
    {
        base.TakeDamage(attacker, damage);
        if (data.shield >= damage)
        {
            data.shield -= damage;
            return;
        }
        damage -= data.shield;
        data.health -= damage;
        if (data.health <= 0)
        {
            Die();
        }
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
        data.health = Mathf.Max(data.health + heal, data.maxHealth.value);
    }

    public override void GainArmour(int amount)
    {
        base.GainArmour(amount);
    }

    public override void Die()
    {
        base.Die();
        Location.units.Remove(this);
        Destroy(gameObject);
    }

    public override bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && cell.units.Count == 0;
    }

    public virtual void OnAbilityCasted(CatAbility abiltiy)
    {
        battleMap.EndTurn();
    }

    public void SetTurnActive(bool active)
    {
        Color color = PlayerColors.Get(owner);
        if (!active)
        {
            color.a = 0.35f;
        }
        ChangePlayerMarkerColor(color);
    }

    public virtual void OnEncounterStart()
    {
        foreach (StatusEffect statusEffect in statusEffects)
        {
            statusEffect.OnEncounterStart(this);
        }
    }

    public override void AddStatusEffect(StatusEffect effect)
    {
        base.AddStatusEffect(effect);
        CalculateStats();
    }

    public override void RemoveStatusEffect(Type type)
    {
        base.RemoveStatusEffect(type);
        CalculateStats();
    }
}
