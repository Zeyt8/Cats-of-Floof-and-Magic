using System;
using System.IO;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerLobby : MonoBehaviour
{
    [SerializeField] private PlayerUIItem playerUIItemPrefab;
    [SerializeField] private Transform playerUIItemParent;
    [SerializeField] private MapUIItem mapUIItemPrefab;
    [SerializeField] private Transform mapsListContent;
    [SerializeField] private TextMeshProUGUI lobbyCode;

    private void OnEnable()
    {
        SetPlayers();
        FillList();
        lobbyCode.text = LobbyHandler.JoinedLobby.LobbyCode;
        LobbyCallbacks.OnLobbyRefresh += SetPlayers;
    }

    private void OnDisable()
    {
        LobbyCallbacks.OnLobbyRefresh -= SetPlayers;
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
        string[] paths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Maps"), "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            MapUIItem item = Instantiate(mapUIItemPrefab, mapsListContent, false);
            item.playerLobby = this;
            item.Name = Path.GetFileNameWithoutExtension(paths[i]);
        }
    }

    private void SetPlayers()
    {
        foreach (Transform child in playerUIItemParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Player t in LobbyHandler.JoinedLobby.Players)
        {
            PlayerUIItem item = Instantiate(playerUIItemPrefab, playerUIItemParent);
            item.Set(t.Data["PlayerName"].Value, Color.red);
        }
    }
}
