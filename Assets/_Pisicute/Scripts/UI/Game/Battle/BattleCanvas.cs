using System.Collections.Generic;
using UnityEngine;

public class BattleCanvas : Singleton<BattleCanvas>
{
    [SerializeField] private Transform abilityPanel;
    [SerializeField] private CatAbilityIcon abilityIconPrefab;
    [SerializeField] private FactionIcon factionIconPrefab;
    [SerializeField] private Transform factionsTransform;
    [SerializeField] private CatIcon catIcon;
    [SerializeField] private StatsPanel statsPanel;
    [SerializeField] private UnitStatusEffectsPanel unitStatusEffectsPanel;
    [SerializeField] private CatIcon catTurnOrderIcon;
    [SerializeField] private GameObject turnOrderPanel;

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
        foreach (Transform t in abilityPanel)
        {
            Destroy(t.gameObject);
        }
        if (cat == null)
        {
            catIcon.SetIcon(null, "");
            statsPanel.Empty();
            unitStatusEffectsPanel.SetStatusEffects(null);
        }
        else
        {
            catIcon.SetIcon(cat.icon, cat.data.type.GetPrettyName());
            for (int i = 0; i < cat.abilities.Count; i++)
            {
                Instantiate(abilityIconPrefab, abilityPanel).Initialize(cat, cat.abilities[i], i);
            }
            statsPanel.SetStats(cat.data);
            unitStatusEffectsPanel.SetStatusEffects(cat);
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

    public void SetTurnOrder(List<Cat> cats)
    {
        foreach (Transform t in turnOrderPanel.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Cat cat in cats)
        {
            Instantiate(catTurnOrderIcon, turnOrderPanel.transform).SetIcon(cat.icon, cat.data.type.GetPrettyName());
        }
    }
}
