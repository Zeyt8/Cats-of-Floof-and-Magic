using UnityEngine;

public class SpellBook : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private SpellEntry spellEntryPrefab;
    [SerializeField] private SpellGraphicsContainer spellGraphicsContainer;

    private void OnEnable()
    {
        inputHandler.OnCancel.AddListener(Close);
    }

    private void OnDisable()
    {
        inputHandler.OnCancel.RemoveListener(Close);
    }

    public void Open(Leader leader)
    {
        gameObject.SetActive(true);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Spell spell in leader.spells)
        {
            SpellEntry entry = Instantiate(spellEntryPrefab, transform);
            entry.SetSpell(spell, this, leader, spellGraphicsContainer[spell.description]);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
