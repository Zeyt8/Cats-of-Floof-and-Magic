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

    public void SetInformation(Lobby lobby)
    {
        this.lobby = lobby;
        sessionNameText.text = lobby.Name;
        sessionPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Deselect()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
    }

    public void SelectSession()
    {
        lobbyUIHandler.DeselectAll();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        MainMenuManager.Instance.SetCurrentSelectedLobby(lobby);
    }
}
