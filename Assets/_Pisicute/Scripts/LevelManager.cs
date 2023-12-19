using System.Collections;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkSingleton<LevelManager>
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
    public SpellBook spellBook;
    public HexGrid CurrentMap => currentBattleMap == null ? mapHexGrid : currentBattleMap.hexGrid;
    [HideInInspector] public BattleMap currentBattleMap;
    public static bool IsBattleActive => Instance.currentBattleMap != null;

    public override void Awake()
    {
        base.Awake();
        Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        string mapName = Path.Combine(Application.streamingAssetsPath, "Maps", GameManager.SelectedMap.Value + ".map");
        using BinaryReader reader = new BinaryReader(File.OpenRead(mapName));
        int header = reader.ReadInt32();
        if (header == 1)
        {
            mapHexGrid.Load(reader, header, mapHexGrid);
        }
        else
        {
            Debug.LogWarning("Unknown map format " + header);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        if (currentPlayer != PlayerObject.Instance.playerNumber) return;
        EndTurnClientRpc();
    }

    [ClientRpc]
    private void EndTurnClientRpc()
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
