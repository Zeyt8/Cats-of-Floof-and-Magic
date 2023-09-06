using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System.Threading.Tasks;

public static class LobbyUtils
{
    public static async Task UpdateData(this Lobby lobby, string playerId, int playerIndex)
    {
        await lobby.UpdateData(new Dictionary<string, int> { { playerId, playerIndex} });
    }

    public static async Task UpdateData(this Lobby lobby, Dictionary<string, int> playerIds)
    {
        UpdateLobbyOptions options = new UpdateLobbyOptions();
        options.Name = lobby.Name;
        options.MaxPlayers = lobby.MaxPlayers;
        options.IsPrivate = lobby.IsPrivate;
        options.HostId = AuthenticationService.Instance.PlayerId;
        options.Data = lobby.Data;
        foreach (var id in playerIds)
        {
            options.Data[id.Key] = new DataObject(DataObject.VisibilityOptions.Member, id.Value.ToString());
        }
        await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
    }
}
