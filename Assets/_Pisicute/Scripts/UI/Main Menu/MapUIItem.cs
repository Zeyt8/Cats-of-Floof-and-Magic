using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetGroup(SingleSelectGroup group)
    {
        group.images.Add(GetComponent<Image>());
        GetComponent<Button>().onClick.AddListener(() => group.Select(gameObject));
    }
}
