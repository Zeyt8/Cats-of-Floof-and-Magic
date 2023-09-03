using TMPro;
using UnityEngine;

public class MapUIItem : MonoBehaviour
{
    [HideInInspector] public PlayerLobby playerLobby;
    [SerializeField] private TextMeshProUGUI mapName;
    public string Name
    {
        get => mapName.text;
        set => mapName.text = value;
    }

    public void SelectMap()
    {
        playerLobby.SetSelectedMap(Name);
    }
}
