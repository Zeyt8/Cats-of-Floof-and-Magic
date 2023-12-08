using System;

public class PassAbility : CatAbility
{
    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => false;
    }
}
