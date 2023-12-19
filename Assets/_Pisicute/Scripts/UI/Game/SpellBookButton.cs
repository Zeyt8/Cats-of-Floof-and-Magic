using UnityEngine;

public class SpellBookButton : MonoBehaviour
{
    public Leader leader;

    public void OpenSpellBook()
    {
        LevelManager.Instance.spellBook.Open(leader);
    }
}
