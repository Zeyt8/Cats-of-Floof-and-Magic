using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIItem : MonoBehaviour
{
    [HideInInspector] public LobbyList lobbyUIHandler;
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI sessionPlayersText;
    private Image image;

    private Lobby lobby;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetInformation(Lobby lobby, SingleSelectGroup singleSelectGroup = null)
    {
        this.lobby = lobby;
        sessionNameText.text = lobby.Name;
        sessionPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        if (singleSelectGroup != null)
        {
            singleSelectGroup.images.Add(image);
            GetComponent<Button>().onClick.AddListener(() => singleSelectGroup.Select(gameObject));
        }
    }

    public void SelectSession()
    {
        PlayPanel.Instance.SetCurrentSelectedLobby(lobby);
    }
}
