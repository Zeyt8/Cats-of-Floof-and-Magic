using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadItem : MonoBehaviour
{
    public SaveLoadMenu Menu;
    [SerializeField] TextMeshProUGUI _text;
    private string _mapName;

    public string MapName
    {
        get => _mapName;
        set
        {
            _mapName = value;
            _text.text = value;
        }
    }


    public void Select()
    {
        Menu.SelectItem(_mapName);
    }
}