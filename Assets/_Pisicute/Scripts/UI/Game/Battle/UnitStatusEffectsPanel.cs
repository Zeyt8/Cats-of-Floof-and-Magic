using TMPro;
using UnityEngine;

public class UnitStatusEffectsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetStatusEffects(UnitObject unit)
    {
        if (unit == null)
        {
            text.text = "";
        }
        else if (unit.statusEffects.Count == 0)
        {
            text.text = "<B>No Status Effects</b>";
        }
        else
        {
            text.text = "";
            for (int i = 0; i < unit.statusEffects.Count; i++)
            {
                text.text += $"<b>{unit.statusEffects[i].Name}</b>\n";
                if (!unit.statusEffects[i].isInfinite)
                {
                    text.text += $"{unit.statusEffects[i].duration} turns remaining.\n";
                }
                text.text += $"{unit.statusEffects[i].Description}";
                if (i != unit.statusEffects.Count - 1)
                {
                    text.text += "\n";
                }
            }
        }
    }
}
