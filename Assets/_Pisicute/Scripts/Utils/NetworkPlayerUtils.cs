using Unity.Services.Lobbies.Models;

public static class NetworkPlayerUtils
{
    public static string GetPlayerName(this Player player)
    {
        return player.Data["PlayerName"].Value;
    }

    public static int GetPlayerIndex(this Player player)
    {
        return LobbyHandler.JoinedLobby.Data.ContainsKey(player.Id) ? int.Parse(LobbyHandler.JoinedLobby.Data[player.Id].Value) : 1;
    }
}
