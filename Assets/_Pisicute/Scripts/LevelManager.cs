using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public int currentPlayer;
    public int maxPlayers = 2;
    public HexGrid mapHexGrid;
    [SerializeField] private CameraController cameraController;
    [Header("UI References")]
    public ResourcesPanel resourcesPanel;
    public BuildingDetails buildingDetails;
    public LeaderDetails unitDetails;
    public Canvas canvas;
    public HexGrid CurrentMap => currentBattleMap == null ? mapHexGrid : currentBattleMap.hexGrid;
    [HideInInspector] public BattleMap currentBattleMap;

    public override void Awake()
    {
        base.Awake();
        Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
    }

    public void EndTurn()
    {
        GameEvents.OnTurnEnd?.Invoke(currentPlayer);
        currentPlayer = currentPlayer + 1;
        if (currentPlayer > maxPlayers)
        {
            currentPlayer = 1;
            GameEvents.OnRoundEnd?.Invoke();
        }
        GameEvents.OnTurnStart?.Invoke(currentPlayer);
    }

    public void MoveCamera(Vector2 position)
    {
        cameraController.transform.position = new Vector3(position.x, 0, position.y);
    }

    public void GoToBattleMap(BattleMap map)
    {
        canvas.gameObject.SetActive(false);
        map.SetBattleActive(true);
        BattleCanvas.Instance.Setup(map);
        currentBattleMap = map;
    }

    public void GoToWorldMap()
    {
        canvas.gameObject.SetActive(true);
        BattleCanvas.Instance.gameObject.SetActive(false);
        currentBattleMap.SetBattleActive(false);
        currentBattleMap = null;
    }
}
