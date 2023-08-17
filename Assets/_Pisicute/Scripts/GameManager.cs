using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public static UnityEvent<int> OnTurnStart = new UnityEvent<int>();
    public static UnityEvent<int> OnTurnEnd = new UnityEvent<int>();
    public static UnityEvent OnRoundEnd = new UnityEvent();

    public int currentPlayer;
    public HexGrid mapHexGrid;
    [Header("UI References")]
    public ResourcesPanel resourcesPanel;
    public BuildingDetails buildingDetails;
    public LeaderDetails unitDetails;
    public Canvas canvas;

    public override void Awake()
    {
        base.Awake();
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
