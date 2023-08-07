using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public int currentPlayer;
    public HexGrid mapHexGrid;
    [Header("Game Events")]
    public UnityEvent<int> OnTurnStart = new UnityEvent<int>();
    public UnityEvent<int> OnTurnEnd = new UnityEvent<int>();
    public UnityEvent OnRoundEnd = new UnityEvent();
    [Header("UI References")]
    public ResourcesPanel resourcesPanel;
    public BuildingDetails buildingDetails;
    public UnitDetails unitDetails;

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
