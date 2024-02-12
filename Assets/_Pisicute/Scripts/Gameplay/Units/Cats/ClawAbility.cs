using System;

public class ClawAbility : CatAbility
{
    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            if (cell.Unit && cell.Unit.owner != cat.owner && cat.Location.GetNeighborDirection(cell).HasValue)
            {
                return true;
            }
            return false;
        };
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return (cell) =>
        {
            caster.DealDamage(cell.Unit, caster.data.power.value);
            EndTurn(caster);
        };
    }
}
