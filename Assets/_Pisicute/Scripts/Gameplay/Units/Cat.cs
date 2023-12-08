using System.Collections.Generic;
using UnityEngine;

public class Cat : UnitObject
{
    public CatData data;
    public Resources sellCost;
    public override int Speed => data.speed;
    public List<CatAbility> abilities;
    [HideInInspector] public BattleMap battleMap;

    protected override void Start()
    {
        base.Start();
        SetTurnActive(false);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        data.health -= damage;
        if (data.health <= 0)
        {
            Die();
        }
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
}
