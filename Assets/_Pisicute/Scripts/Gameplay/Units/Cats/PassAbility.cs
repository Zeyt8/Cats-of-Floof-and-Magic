using JSAM;
using System;

public class PassAbility : CatAbility
{
    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) => cell == cat.Location;
    }

    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        EndTurn(caster);
        AudioManager.PlaySound(AudioLibrarySounds.Pass);
        return (cell) => { };
    }
}
