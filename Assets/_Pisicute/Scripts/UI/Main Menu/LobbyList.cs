using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private GameObject lobbyItemListPrefab;
    [SerializeField] private Transform lobbyListParent;

    List<LobbyUIItem> lobbyUIItems = new List<LobbyUIItem>();

    private void OnEnable()
    {
        LobbyCallbacks.OnLobbyListUpdated += AddSessionsToList;
    }

    private void OnDisable()
    {
        LobbyCallbacks.OnLobbyListUpdated -= AddSessionsToList;
    }

    private void AddSessionsToList(List<Lobby> lobbies)
    {
        ClearList();
        foreach (Lobby lobby in lobbies)
        {
            AddSessionToList(lobby);
        }
    }

    private void ClearList()
    {
        foreach (Transform child in lobbyListParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void DeselectAll()
    {
        foreach (LobbyUIItem l in lobbyUIItems)
        {
            l.Deselect();
        }
    }

    private void AddSessionToList(Lobby lobby)
    {
        LobbyUIItem sessionItem = Instantiate(lobbyItemListPrefab, lobbyListParent).GetComponent<LobbyUIItem>();
        sessionItem.lobbyUIHandler = this;
        sessionItem.SetInformation(lobby);
        lobbyUIItems.Add(sessionItem);
    }
}
