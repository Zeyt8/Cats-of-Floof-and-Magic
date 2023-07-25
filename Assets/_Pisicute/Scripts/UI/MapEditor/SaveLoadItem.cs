using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadItem : MonoBehaviour
{
    public SaveLoadMenu menu;
    [SerializeField] TextMeshProUGUI text;
    private string mapName;

    public string MapName
    {
        get => mapName;
        set
        {
            mapName = value;
            text.text = value;
        }
    }


    public void Select()
    {
        menu.SelectItem(mapName);
    }
}