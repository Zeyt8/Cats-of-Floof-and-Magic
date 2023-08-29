using UnityEngine;

public class BattleCanvas : Singleton<BattleCanvas>
{
    [SerializeField] private Transform abilityPanel;
    [SerializeField] private CatAbilityIcon abilityIconPrefab;

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
        foreach (CatAbility ability in cat.abilities)
        {
            Instantiate(abilityIconPrefab, abilityPanel).Initialize(cat, ability);
        }
    }

    public void EndTurn()
    {
        battleMap.EndTurn();
    }
}
