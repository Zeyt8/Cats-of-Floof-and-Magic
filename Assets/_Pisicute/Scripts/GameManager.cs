using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int currentPlayer;
    public HexGrid mapHexGrid;
    [SerializeField] private CameraController cameraController;
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
        GameEvents.OnTurnEnd?.Invoke(currentPlayer);
        currentPlayer = (currentPlayer + 1) % 2;
        if (currentPlayer == 0)
        {
            GameEvents.OnRoundEnd?.Invoke();
        }
        GameEvents.OnTurnStart?.Invoke(currentPlayer);
    }

    public void MoveCamera(Vector2 position)
    {
        cameraController.transform.position = new Vector3(position.x, 0, position.y);
    }
}
