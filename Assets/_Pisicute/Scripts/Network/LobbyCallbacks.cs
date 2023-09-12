using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyCallbacks
{
    public static event Action<List<Lobby>> OnLobbyListUpdated;
    public static event Action OnLobbyRefresh;
    public static event Action OnLobbyDisconnect;
    public static event Action<List<LobbyPlayerJoined>> OnPlayersJoined;
    public static event Action<List<int>> OnPlayersLeft;

    private static ILobbyEvents LobbyEvents;

    public static void ResetCallbacks()
    {
        OnLobbyListUpdated = null;
        OnLobbyRefresh = null;
        OnLobbyDisconnect = null;
        OnPlayersJoined = null;
        OnPlayersLeft = null;
    }

    public static async void RefreshLobbyList()
    {
        List<Lobby> lobbies = await LobbyHandler.ListLobbies();
        OnLobbyListUpdated?.Invoke(lobbies);
    }

    public static async Task SubscribeToLobbyChanges()
    {
        LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
        callbacks.PlayerJoined += OnPlayerJoined;
        callbacks.PlayerLeft += OnPlayerLeft;
        callbacks.LobbyChanged += OnLobbyChanged;
        callbacks.KickedFromLobby += OnKickedFromLobby;
        callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
        try
        {
            LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(LobbyHandler.JoinedLobby.Id, callbacks);
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{LobbyHandler.JoinedLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                default: throw;
            }
        }
    }

    private static void OnPlayerJoined(List<LobbyPlayerJoined> players)
    {
        OnPlayersJoined?.Invoke(players);
    }

    private static void OnPlayerLeft(List<int> players)
    {
        OnPlayersLeft?.Invoke(players);
    }

    private static void OnLobbyChanged(ILobbyChanges changes)
    {
        if (changes.LobbyDeleted)
        {
            OnLobbyDisconnect?.Invoke();
            OnLobbyDisconnect = null;
            RefreshLobbyList();
        }
        else
        {
            changes.ApplyToLobby(LobbyHandler.JoinedLobby);
            OnLobbyRefresh?.Invoke();
        }
    }

    private static void OnKickedFromLobby()
    {
        OnLobbyDisconnect?.Invoke();
        LobbyEvents = null;
        OnLobbyDisconnect = null;
        RefreshLobbyList();
    }

    private static void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
    {
        switch (state)
        {
            case LobbyEventConnectionState.Unsubscribed: /* Update the UI if necessary, as the subscription has been stopped. */ break;
            case LobbyEventConnectionState.Subscribing: /* Update the UI if necessary, while waiting to be subscribed. */ break;
            case LobbyEventConnectionState.Subscribed: /* Update the UI if necessary, to show subscription is working. */ break;
            case LobbyEventConnectionState.Unsynced: /* Update the UI to show connection problems. Lobby will attempt to reconnect automatically. */ break;
            case LobbyEventConnectionState.Error: OnLobbyDisconnect?.Invoke(); break;
        }
    }
}
