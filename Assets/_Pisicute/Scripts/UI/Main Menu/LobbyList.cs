using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private GameObject lobbyItemListPrefab;
    [SerializeField] private Transform lobbyListParent;

    private List<LobbyUIItem> lobbyUIItems = new List<LobbyUIItem>();
    private SingleSelectGroup singleSelectGroup;

    private void Awake()
    {
        singleSelectGroup = GetComponent<SingleSelectGroup>();
    }

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
        singleSelectGroup.images.Clear();
        foreach (Transform child in lobbyListParent)
        {
            Destroy(child.gameObject);
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
