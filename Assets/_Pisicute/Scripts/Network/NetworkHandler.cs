using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworkHandler : Singleton<NetworkHandler>
{
    public static string PlayerName;
    public static string PlayerId => AuthenticationService.Instance.PlayerId;

    private float heartbeatTimer;
    private float lobbiesUpdateTimer;

    private void OnDisable()
    {
        LobbyCallbacks.ResetCallbacks();
    }

    private async void Update()
    {
        if (LobbyHandler.IsLobbyHost)
        {
            heartbeatTimer += Time.deltaTime;
            if (heartbeatTimer > 20)
            {
                heartbeatTimer = 0;
                await LobbyService.Instance.SendHeartbeatPingAsync(LobbyHandler.JoinedLobby.Id);
            }
        }

        lobbiesUpdateTimer += Time.deltaTime;
        if (UnityServices.State == ServicesInitializationState.Initialized && lobbiesUpdateTimer > 30)
        {
            lobbiesUpdateTimer = 0;
            LobbyCallbacks.RefreshLobbyList();
        }
    }

    public static Player GetPlayer()
    {
        return new Player
        (
            data: new Dictionary<string, PlayerDataObject>
            {
                {
                    "PlayerName",
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName)
                },
                {
                    "PlayerIndex",
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "1")
                }
            }
        );
    }

    public static async Task ConnectToServer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            // To be able to use lobby with 2 local instances
#if UNITY_EDITOR
            AuthenticationService.Instance.ClearSessionToken();
#endif
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public static void DisconnectFromServer()
    {
        AuthenticationService.Instance.SignOut();
    }
}
