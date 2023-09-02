using UnityEngine;
using UnityEngine.UI;

public class CatAbilityIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    private CatAbility ability;
    private Cat cat;

    public void Initialize(Cat cat, CatAbility catAbility)
    {
        this.cat = cat;
        ability = catAbility;
        icon.sprite = ability.icon;
    }

    public void BeginAbility()
    {
        PlayerObject.Instance.InitiateSelectCellForEffect(ability.GetAvailableTargets(cat), ability.CastAbility(cat));
    }
}
