using JSAM;
using UnityEngine;

public class MageGuild : Building
{
    private Spell SpellFactory(int random)
    {
        return random switch
        {
            0 => new ConjureFoodSpell(),
            1 => new TeleportSpell(),
        };
    }

    public override void OnSpawn(HexCell cell)
    {
        base.OnSpawn(cell);
        action = () =>
        {
            var spell = SpellFactory(Random.Range(0, 2));
            ((Leader)Location.Unit).AddSpell(spell);
            action = null;
        };
    }

    public override void OnSelect()
    {
        base.OnSelect();
        AudioManager.PlaySound(AudioLibrarySounds.MageGuild);
    }
}
