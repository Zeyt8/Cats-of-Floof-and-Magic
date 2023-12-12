using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatAbilityIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;
    private CatAbility ability;
    private Cat cat;

    public void Initialize(Cat cat, CatAbility catAbility)
    {
        this.cat = cat;
        ability = catAbility;
        icon.sprite = ability.icon;
        description.text = $"<b>{catAbility.title}</b>\n{catAbility.description}";
    }

    public void BeginAbility()
    {
        if (ability.activationType == CatAbility.ActivationType.Instant)
        {
            ability.CastAbility(cat)(cat.Location);
        }
        else if (ability.activationType == CatAbility.ActivationType.RequiresTarget)
        {
            PlayerObject.Instance.InitiateSelectCellForEffect(ability.GetAvailableTargets(cat), ability.CastAbility(cat));
        }
    }
}
