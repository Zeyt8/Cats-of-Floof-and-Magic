using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Spell spell;
    private SpellBook spellBook;
    private Leader caster;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetSpell(Spell spell, SpellBook spellBook, Leader caster)
    {
        text.text = $"{spell.description}: <color=#BE21BA>{spell.floofCost} floof</color>\nCooldown: {spell.baseCooldown} turn(s); {spell.cooldown} remaining";
        this.spell = spell;
        this.spellBook = spellBook;
        this.caster = caster;
        if (spell.cooldown > 0 || caster.currentFloof < spell.floofCost)
        {
            image.color = Color.gray;
        }
        else
        {
            image.color = Color.white;
        }
    }

    public void OnClick()
    {
        if (spell.cooldown > 0 || caster.currentFloof < spell.floofCost) return;
        spellBook.Close();
        PlayerObject.Instance.InitiateSelectCellForEffect(spell.GetAvailableTargets(caster), spell.CastAbility(caster));
    }
}
