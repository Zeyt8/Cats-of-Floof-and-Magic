using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatAbilityIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button button;
    private CatAbility ability;
    private Cat cat;
    private int cooldownIndex;

    public void Initialize(Cat cat, CatAbility catAbility, int index)
    {
        this.cat = cat;
        ability = catAbility;
        icon.sprite = ability.icon;
        cooldownIndex = index;
        description.text = $"<b>{catAbility.title}</b>\n" +
            $"{(cat.abilityCooldownRemaining[cooldownIndex] > 0 ? cat.abilityCooldownRemaining[cooldownIndex] + " turns left" : ability.cooldown + " turns cooldown")}\n" +
            $"{catAbility.description}";
    }

    private void Update()
    {
        button.interactable = cat.abilityCooldownRemaining[cooldownIndex] <= 0 && cat.isActive && cat.owner == PlayerObject.Instance.playerNumber;
    }

    public void BeginAbility()
    {
        if (cat.abilityCooldownRemaining[cooldownIndex] > 0) return;
        if (ability.activationType == CatAbility.ActivationType.Instant)
        {
            ability.CastAbility(cat)(cat.Location);
            ability.AfterCasting(cat)(cat.Location);
        }
        else if (ability.activationType == CatAbility.ActivationType.RequiresTarget)
        {
            PlayerObject.Instance.InitiateSelectCellForEffect(ability.GetAvailableTargets(cat), ability.CastAbility(cat), ability.AfterCasting(cat));
        }
    }
}
