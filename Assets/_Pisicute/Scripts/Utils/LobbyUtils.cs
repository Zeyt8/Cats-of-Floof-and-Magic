using System.Collections.Generic;
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
        options.Data = lobby.Data;
        // check if any player id changed
        bool changed = false;
        foreach (var id in playerIds)
        {
            if (!lobby.Data.ContainsKey(id.Key) || id.Value != int.Parse(lobby.Data[id.Key].Value))
            {
                changed = true;
                options.Data[id.Key] = new DataObject(DataObject.VisibilityOptions.Member, id.Value.ToString());
            }
        }
        if (!changed)
        {
            return;
        }
        await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
    }
}
