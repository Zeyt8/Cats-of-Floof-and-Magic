using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private LoadingPanel loadingPanel;

    public async void ConnectToServer()
    {
        loadingPanel.ShowLoad(LoadingType.Connecting);
        try
        {
            await NetworkHandler.ConnectToServer();
            loadingPanel.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            loadingPanel.ShowLoad(LoadingType.Error, e.Message);
        }
    }

    public void DisconnectFromServer()
    {
        NetworkHandler.DisconnectFromServer();
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
