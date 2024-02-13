using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkPersistentSingleton<GameManager>
{
    public static NetworkVariable<FixedString128Bytes> SelectedMap = new NetworkVariable<FixedString128Bytes>();
    public static int winningPlayer;

    public static void EndGame(int winningPlayer)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("EndScreen", LoadSceneMode.Single);
        GameManager.winningPlayer = winningPlayer;
    }
}
