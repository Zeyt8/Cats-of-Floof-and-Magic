using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : NetworkSingleton<MainMenuManager>
{
    public string levelScene;
    [Header("UI Elements")]
    [SerializeField] private LoadingPanel loadingPanel;
    [SerializeField] private TMP_InputField createLobbyNameInput;
    [SerializeField] private TMP_InputField joinPrivateLobbyCodeInput;
    [SerializeField] private Toggle privateLobbyToggle;
    [SerializeField] private PlayerLobby playerLobby;

    private Lobby currentSelectedLobby;

    private void Start()
    {
        ConnectToServer();
    }

    public void SetPlayerName(string playerName)
    {
        NetworkHandler.PlayerName = playerName;
    }

    public async void ConnectToServer()
    {
        loadingPanel.ShowLoad(LoadingType.Connecting);
        try
        {
            await NetworkHandler.ConnectToServer();
            loadingPanel.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            loadingPanel.ShowLoad(LoadingType.Error, e.Message);
        }
    }

    public async void JoinLobby()
    {
        loadingPanel.ShowLoad(LoadingType.JoiningRoom);
        try
        {
            NetworkManager.Singleton.Shutdown();
            await LobbyHandler.JoinLobby(currentSelectedLobby.Id, JoinLobbyMode.Id);
            loadingPanel.gameObject.SetActive(false);

            NetworkManager.Singleton.StartClient();
            playerLobby.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            if (currentSelectedLobby == null)
                loadingPanel.ShowLoad(LoadingType.Error, "No lobby selected");
            else
                loadingPanel.ShowLoad(LoadingType.Error, e.Message);
        }
    }

    public async void JoinPrivateLobby()
    {
        loadingPanel.ShowLoad(LoadingType.JoiningRoom);
        try
        {
            NetworkManager.Singleton.Shutdown();
            await LobbyHandler.JoinLobby(joinPrivateLobbyCodeInput.text, JoinLobbyMode.Code);
            loadingPanel.gameObject.SetActive(false);

            NetworkManager.Singleton.StartClient();
            playerLobby.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            if (string.IsNullOrEmpty(joinPrivateLobbyCodeInput.text))
                loadingPanel.ShowLoad(LoadingType.Error, "Please enter a lobby code");
            else
                loadingPanel.ShowLoad(LoadingType.Error, e.Message);
        }
    }

    public async void CreateLobby()
    {
        loadingPanel.ShowLoad(LoadingType.CreatingRoom);
        try
        {
            NetworkManager.Singleton.Shutdown();
            await LobbyHandler.CreateLobby(createLobbyNameInput.text, privateLobbyToggle.isOn);
            loadingPanel.gameObject.SetActive(false);
            NetworkManager.Singleton.StartHost();
            playerLobby.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            if (string.IsNullOrEmpty(createLobbyNameInput.text))
                loadingPanel.ShowLoad(LoadingType.Error, "Please enter a lobby name");
            else
                loadingPanel.ShowLoad(LoadingType.Error, e.Message);
        }
    }

    public void SetCurrentSelectedLobby(Lobby lobby)
    {
        currentSelectedLobby = lobby;
    }

    public async void LeaveLobby()
    {
        try
        {
            NetworkManager.Singleton.Shutdown();
            if (LobbyHandler.IsLobbyHost)
            {
                await LobbyHandler.DeleteLobby();
            }
            else
            {
                await LobbyHandler.LeaveLobby();
            }
            playerLobby.gameObject.SetActive(false);
            RefreshLobbyList();
        }
        catch (Exception e)
        {
            loadingPanel.ShowLoad(LoadingType.Error, e.Message);
        }
    }

    public void RefreshLobbyList()
    {
        LobbyCallbacks.RefreshLobbyList();
    }

    public void DisconnectFromServer()
    {
        NetworkHandler.DisconnectFromServer();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
