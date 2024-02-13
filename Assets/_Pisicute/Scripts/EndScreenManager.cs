using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        /*string playerName = "";
        foreach (var player in LobbyHandler.JoinedLobby.Players)
        {
            foreach (var data in LobbyHandler.JoinedLobby.Data)
            {
                if (data.Key == player.Id)
                {
                    if (int.Parse(data.Value.Value) == GameManager.winningPlayer)
                    {
                        playerName = player.GetPlayerName();
                    }
                }
            }
            
        }*/
        text.text = $"<color=#{PlayerColors.Get(GameManager.winningPlayer).ToHexString()}>{PlayerColors.GetName(GameManager.winningPlayer)}</color> player wins!";
    }

    public async void ReturnToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        if (LobbyHandler.IsLobbyHost)
        {
            await LobbyHandler.DeleteLobby();
        }
        else
        {
            await LobbyHandler.LeaveLobby();
        }
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
