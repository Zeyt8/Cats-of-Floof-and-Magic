using System;

public class PallasPounce : CatAbility
{
    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return (cell) => caster.Location = cell;
    }

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => true;
    }
}
