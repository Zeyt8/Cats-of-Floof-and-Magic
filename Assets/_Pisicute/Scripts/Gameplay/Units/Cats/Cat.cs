using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Cat : UnitObject
{
    public CatData data;
    public Resources sellCost;
    public override int Speed => data.speed.value;
    public List<CatAbility> abilities;
    [HideInInspector] public List<int> abilityCooldownRemaining;
    [HideInInspector] public Leader leader;
    [HideInInspector] public BattleMap battleMap;

    public bool isActive { get; private set; }
    private int aiState;

    protected override void Start()
    {
        base.Start();
        CalculateStats();
        SetTurnActive(false);
        abilityCooldownRemaining = Enumerable.Repeat(0, abilities.Count).ToList();
    }

    private void Update()
    {
        if (isActive && owner == 0)
        {
            if (aiState == 0)
            {
                List<Cat> enemyArmy = battleMap.GetOpponentArmy(owner);
                HexCell closestEnemy = null;
                int minDistance = int.MaxValue;
                foreach (Cat enemy in enemyArmy)
                {
                    int distance = Location.coordinates.DistanceTo(enemy.Location.coordinates);
                    if (closestEnemy == null || distance < minDistance)
                    {
                        closestEnemy = enemy.Location;
                        minDistance = distance;
                    }
                }
                minDistance = int.MaxValue;
                HexCell targetCell = closestEnemy;
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbour = closestEnemy.GetNeighbor(d);
                    if (neighbour == null) continue;
                    int distance = neighbour.coordinates.DistanceTo(Location.coordinates);
                    if (IsValidDestination(neighbour) && distance < minDistance)
                    {
                        targetCell = neighbour;
                        minDistance = distance;
                    }
                }
                battleMap.hexGrid.FindPath(Location, targetCell, this, false);
                List<HexCell> path = battleMap.hexGrid.GetPath();
                if (path != null)
                {
                    Travel(path);
                }
                aiState = 1;
            }
            else if (aiState == 1)
            {
                if (!IsMoving)
                {
                    for (int i = abilities.Count - 1; i >= 0; i--)
                    {
                        if (abilityCooldownRemaining[i] <= 0)
                        {
                            var available = abilities[i].GetAvailableTargets(this);
                            foreach (HexCell cell in battleMap.hexGrid.cells)
                            {
                                if (available(cell))
                                {
                                    abilities[i].CastAbility(this)(cell);
                                    abilities[i].AfterCasting(this)(cell);
                                }
                            }
                        }
                    }
                    aiState = 0;
                }
            }
        }
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

    public override void DealDamage(UnitObject target, ref int damage)
    {
        base.DealDamage(target, ref damage);
        PlayerObject.Instance.DealDamageToCatServerRpc(damage, BattleManager.GetBattleMapIndex(battleMap), target.Location.coordinates, target.owner);
    }

    public override void TakeDamage(ref int damage)
    {
        base.TakeDamage(ref damage);
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

    [ContextMenu("Die")]
    public override void Die()
    {
        base.Die();
        Location.units.Remove(this);
        if (leader)
        {
            leader.army.Remove(data);
            leader.currentArmy.Remove(this);
        }
        Destroy(gameObject);
    }

    public override bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && cell.units.Count == 0;
    }

    public void SetTurnActive(bool active)
    {
        Color color = PlayerColors.Get(owner);
        if (!active)
        {
            color.a = 0.35f;
        }
        ChangePlayerMarkerColor(color);
        isActive = active;
        if (active)
        {
            for (int i = 0; i < abilityCooldownRemaining.Count; i++)
            {
                abilityCooldownRemaining[i]--;
            }
        }
    }

    public virtual void OnEncounterStart()
    {
        foreach (StatusEffect statusEffect in statusEffects)
        {
            statusEffect.OnEncounterStart(this);
        }
        abilityCooldownRemaining = Enumerable.Repeat(0, abilities.Count).ToList();
        for (int i = 0; i < abilityCooldownRemaining.Count; i++)
        {
            abilityCooldownRemaining[i] = 0;
        }
    }

    public override void AddStatusEffect(StatusEffect effect)
    {
        base.AddStatusEffect(effect);
        CalculateStats();
    }

    public override void RemoveStatusEffect(FixedString32Bytes type)
    {
        base.RemoveStatusEffect(type);
        CalculateStats();
    }
}
