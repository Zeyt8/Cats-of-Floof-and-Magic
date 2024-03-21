using System;
using UnityEngine;

public class PallasPounce : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private float damageModifier;
    [SerializeField] private GameObject pounceGraphics;

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return (cell) =>
        {
            HexCell pounceLocation = null;
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (caster.IsValidDestination(neighbor) && neighbor.GetEdgeType(d) != HexEdgeType.Cliff)
                {
                    pounceLocation = neighbor;
                    break;
                }
            }
            if (pounceLocation == null)
            {
                return;
            }
            Instantiate(pounceGraphics, caster.transform.position, Quaternion.identity);
            caster.Location = pounceLocation;
            int damage = (int)(caster.data.power.value * damageModifier);
            caster.DealDamage(cell.Unit, ref damage);
            EndTurn(caster);
        };
    }

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            return cell.Unit && cell.Unit.owner != cat.owner && cell.coordinates.DistanceTo(cat.Location.coordinates) <= range;
        };
    }
}
