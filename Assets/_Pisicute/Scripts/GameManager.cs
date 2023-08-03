using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public int currentPlayer;
    [Header("Game Events")]
    public UnityEvent<int> OnTurnStart = new UnityEvent<int>();
    public UnityEvent<int> OnTurnEnd = new UnityEvent<int>();
    public UnityEvent OnRoundEnd = new UnityEvent();
    [Header("UI References")]
    public ResourcesPanel resourcesPanel;
    public BuildingDetails buildingDetails;
    public UnitDetails unitDetails;

    // Start is called before the first frame update
    void Start()
    {
        Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke(currentPlayer);
        currentPlayer = (currentPlayer + 1) % 2;
        if (currentPlayer == 0)
        {
            OnRoundEnd?.Invoke();
        }
        OnTurnStart?.Invoke(currentPlayer);
    }
}
