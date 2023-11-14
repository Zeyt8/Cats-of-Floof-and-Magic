using Unity.Services.Lobbies.Models;

public static class NetworkPlayerUtils
{
    public static string GetPlayerName(this Player player)
    {
        return player.Data["PlayerName"].Value;
    }

    public static int GetPlayerIndex(string playerId)
    {
        return LobbyHandler.JoinedLobby.Data.ContainsKey(playerId) ? int.Parse(LobbyHandler.JoinedLobby.Data[playerId].Value) : 0;
    }
}
