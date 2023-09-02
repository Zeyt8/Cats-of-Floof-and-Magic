using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;

public enum JoinLobbyMode
{
    Id,
    Code
}

public class LobbyHandler
{
    public static Lobby JoinedLobby;
    public static bool IsLobbyHost { get; set; }

    public static async Task CreateLobby(string lobbyName, bool isPrivate)
    {
        int maxPlayers = 2;
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            IsPrivate = isPrivate,
            Player = NetworkHandler.GetPlayer(),
            Data = new Dictionary<string, DataObject>
            {
                { "KEY_RELAY", new DataObject(DataObject.VisibilityOptions.Member, "0") }
            }
        };
        try
        {
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            JoinedLobby = lobby;
            IsLobbyHost = true;
            await LobbyCallbacks.SubscribeToLobbyChanges();
            string relay = await RelayHandler.CreateRelay();
            await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "KEY_RELAY", new DataObject(DataObject.VisibilityOptions.Member, relay) }
                }
            });
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public static async Task JoinLobby(string lobbyId, JoinLobbyMode mode)
    {
        try
        {
            Lobby lobby;
            if (mode == JoinLobbyMode.Id)
            {
                JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
                {
                    Player = NetworkHandler.GetPlayer()
                };
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
            }
            else
            {
                JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
                {
                    Player = NetworkHandler.GetPlayer()
                };
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyId, options);
            }

            JoinedLobby = lobby;
            await LobbyCallbacks.SubscribeToLobbyChanges();
            await RelayHandler.JoinRelay(JoinedLobby.Data["KEY_RELAY"].Value);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public static async Task LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
            JoinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public static async Task DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
            JoinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public static async Task<List<Lobby>> ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            return queryResponse.Results;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }
}
