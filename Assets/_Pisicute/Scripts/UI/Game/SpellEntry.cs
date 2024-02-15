using TMPro;
using UnityEngine;

public class SpellEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Spell spell;
    private SpellBook spellBook;
    private Leader caster;

    public void SetSpell(Spell spell, SpellBook spellBook, Leader caster)
    {
        text.text = $"{spell.description}: <color=#BE21BA>{spell.floofCost} floof</color>\nCooldown: {spell.baseCooldown} turn(s); {spell.cooldown} remaining";
        this.spell = spell;
        this.spellBook = spellBook;
        this.caster = caster;
    }

    public void OnClick()
    {
        if (spell.cooldown > 0) return;
        spellBook.Close();
        PlayerObject.Instance.InitiateSelectCellForEffect(spell.GetAvailableTargets(caster), spell.CastAbility(caster));
    }
}
