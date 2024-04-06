using JSAM;
using System;
using UnityEngine;

public class TeleportAbility : CatAbility
{
    [SerializeField] private int range;
    [SerializeField] private GameObject teleportGraphics;

    public override Func<HexCell, bool> GetAvailableTargets(Cat cat)
    {
        return (cell) =>
        {
            return !cell.Unit && cell.coordinates.DistanceTo(cat.Location.coordinates) <= range;
        };
    }
    public override PlayerObject.Action<HexCell> CastAbility(Cat caster)
    {
        return (cell) =>
        {
            Instantiate(teleportGraphics, caster.transform.position, Quaternion.identity);
            caster.Location = cell;
            EndTurn(caster);
            Instantiate(teleportGraphics, cell.transform.position, Quaternion.identity);
            AudioManager.PlaySound(AudioLibrarySounds.Teleport);
        };
    }
}
