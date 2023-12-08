using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvas : Singleton<BattleCanvas>
{
    [SerializeField] private Transform abilityPanel;
    [SerializeField] private CatAbilityIcon abilityIconPrefab;
    [SerializeField] private FactionIcon factionIconPrefab;
    [SerializeField] private Transform factionsTransform;
    [SerializeField] private Image catIcon;

    private BattleMap battleMap;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Setup(BattleMap map)
    {
        battleMap = map;
        gameObject.SetActive(true);
    }

    public void ShowAbilities(Cat cat)
    {
        catIcon.sprite = cat.icon;
        foreach (Transform t in abilityPanel)
        {
            Destroy(t.gameObject);
        }
        foreach (CatAbility ability in cat.abilities)
        {
            Instantiate(abilityIconPrefab, abilityPanel).Initialize(cat, ability);
        }
    }

    public void SetupFactionEffect(List<FactionEffect> effects)
    {
        foreach (FactionEffect faction in effects)
        {
            Instantiate(factionIconPrefab, factionsTransform).Initialize(faction.faction, faction.count, faction.nextThreshold, faction.Title, faction.Description);
        }
    }

    public void EndTurn()
    {
        battleMap.EndTurn();
    }
}
