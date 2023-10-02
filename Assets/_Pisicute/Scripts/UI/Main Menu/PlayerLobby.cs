using System;
using System.Collections.Generic;
using System.IO;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies;

public class PlayerLobby : MonoBehaviour
{
    [SerializeField] private PlayerUIItem playerUIItemPrefab;
    [SerializeField] private Transform playerUIItemParent;
    [SerializeField] private MapUIItem mapUIItemPrefab;
    [SerializeField] private Transform mapsListContent;
    [SerializeField] private TextMeshProUGUI lobbyCode;

    private SingleSelectGroup singleSelectGroup;

    private void Awake()
    {
        singleSelectGroup = GetComponent<SingleSelectGroup>();
    }

    private void OnEnable()
    {
        SetPlayers(null);
        FillList();
        lobbyCode.text = LobbyHandler.JoinedLobby.LobbyCode;
        LobbyCallbacks.LobbyEvents.Callbacks.LobbyChanged += SetPlayers;
    }

    private void OnDisable()
    {
        LobbyCallbacks.LobbyEvents.Callbacks.LobbyChanged -= SetPlayers;
    }

    public void SetSelectedMap(string mapName)
    {
        GameManager.SelectedMap.Value = mapName;
    }

    private void FillList()
    {
        for (int i = 0; i < mapsListContent.childCount; i++)
        {
            Destroy(mapsListContent.GetChild(i).gameObject);
        }
        singleSelectGroup.images.Clear();
        string[] paths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Maps"), "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            MapUIItem item = Instantiate(mapUIItemPrefab, mapsListContent, false);
            item.playerLobby = this;
            item.Name = Path.GetFileNameWithoutExtension(paths[i]);
            item.SetGroup(singleSelectGroup);
        }
    }

    private async void SetPlayers(ILobbyChanges changes)
    {
        foreach (Transform child in playerUIItemParent)
        {
            Destroy(child.gameObject);
        }

        if (LobbyHandler.IsLobbyHost)
        {
            List<Player> players = LobbyHandler.JoinedLobby.Players;
            players.Sort((p1, p2) => p1.Id.CompareTo(p2.Id));
            Dictionary<string, int> playerIds = new Dictionary<string, int>();
            for (int i = 0; i < players.Count; i++)
            {
                playerIds[players[i].Id] = i + 1;
            }
            await LobbyHandler.JoinedLobby.UpdateData(playerIds);
        }

        foreach (Player t in LobbyHandler.JoinedLobby.Players)
        {
            PlayerUIItem item = Instantiate(playerUIItemPrefab, playerUIItemParent);
            item.Set(t.GetPlayerName(), PlayerColors.Get(NetworkPlayerUtils.GetPlayerIndex(t.Id)));
        }
    }
}
