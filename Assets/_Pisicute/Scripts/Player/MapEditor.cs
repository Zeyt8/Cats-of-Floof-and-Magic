using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapEditor : MonoBehaviour
{
    static int CellHighlightingId = Shader.PropertyToID("_CellHighlighting");
    
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private Material terrainMaterial;
    private int activeTerrainTypeIndex = -1;
    private int activeElevation;
    private int activeWaterLevel;
    private int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;
    private bool applyElevation;
    private bool applyWaterLevel;
    private bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;
    private int brushSize;
    private OptionalToggle riverMode;
    private OptionalToggle roadMode;
    private OptionalToggle walledMode;
    private bool isDrag;
    private HexDirection dragDirection;
    private HexCell previousCell;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    private void Awake()
    {
        terrainMaterial.DisableKeyword("_SHOW_GRID");
        Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
        SetEditMode(true);
    }

    private void OnEnable()
    {
        inputHandler.mapEditor.OnCreateUnit.AddListener(CreateUnit);
        inputHandler.mapEditor.OnDestroyUnit.AddListener(DestroyUnit);
    }

    private void OnDisable()
    {
        inputHandler.mapEditor.OnCreateUnit.RemoveListener(CreateUnit);
        inputHandler.mapEditor.OnDestroyUnit.RemoveListener(DestroyUnit);
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (inputHandler.mapEditor.isEditing)
            {
                HandleInput();
            }
            else
            {
                previousCell = null;
            }
            UpdateCellHighlightData(GetCellUnderCursor());
        }
        else
        {
            previousCell = null;
            ClearCellHighlightData();
        }
    }

    private void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    private void EditCell(HexCell cell)
    {
        if (!cell) return;
        if (activeTerrainTypeIndex >= 0)
        {
            cell.TerrainTypeIndex = activeTerrainTypeIndex;
        }
        if (applyElevation)
        {
            cell.Elevation = activeElevation;
        }
        if (applyWaterLevel)
        {
            cell.WaterLevel = activeWaterLevel;
        }
        if (applyUrbanLevel)
        {
            cell.UrbanLevel = activeUrbanLevel;
        }
        if (applyFarmLevel)
        {
            cell.FarmLevel = activeFarmLevel;
        }
        if (applyPlantLevel)
        {
            cell.PlantLevel = activePlantLevel;
        }
        if (applySpecialIndex)
        {
            cell.SpecialIndex = activeSpecialIndex;
        }
        if (riverMode == OptionalToggle.No)
        {
            cell.RemoveRiver();
        }
        if (roadMode == OptionalToggle.No)
        {
            cell.RemoveRoads();
        }
        if (walledMode == OptionalToggle.No)
        {
            cell.RemoveWall();
        }
        if (isDrag)
        {
            HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
            if (otherCell)
            {
                if (riverMode == OptionalToggle.Yes)
                {
                    otherCell.SetOutgoingRiver(dragDirection);
                }
                if (roadMode == OptionalToggle.Yes)
                {
                    otherCell.AddRoad(dragDirection);
                }
                if (walledMode == OptionalToggle.Yes)
                {
                    otherCell.AddWall(dragDirection);
                }
            }
        }
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;
        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private void ValidateDrag(HexCell currentCell)
    {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }
    private HexCell GetCellUnderCursor()
    {
        return hexGrid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
    }

    private void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.unit)
        {
            hexGrid.AddUnit(Instantiate(hexGrid.unitPrefab), cell, Random.Range(0, 360f));
        }
    }

    private void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.unit)
        {
            hexGrid.RemoveUnit(cell.unit);
        }
    }

    void UpdateCellHighlightData(HexCell cell)
    {
        if (cell == null)
        {
            ClearCellHighlightData();
            return;
        }

        Shader.SetGlobalVector(CellHighlightingId, new Vector4(cell.coordinates.HexX, cell.coordinates.HexZ, brushSize * brushSize + 0.5f, 0));
    }

    void ClearCellHighlightData()
    {
        Shader.SetGlobalVector(CellHighlightingId, new Vector4(0f, 0f, -1f, 0f));
    }

    #region UI
    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle)mode;
    }

    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SetApplyUrbanLevel(bool toggle)
    {
        applyUrbanLevel = toggle;
    }

    public void SetUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }

    public void SetApplyFarmLevel(bool toggle)
    {
        applyFarmLevel = toggle;
    }

    public void SetFarmLevel(float level)
    {
        activeFarmLevel = (int)level;
    }

    public void SetApplyPlantLevel(bool toggle)
    {
        applyPlantLevel = toggle;
    }

    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }

    public void SetWalledMode(int mode)
    {
        walledMode = (OptionalToggle)mode;
    }

    public void SetApplySpecialIndex(bool toggle)
    {
        applySpecialIndex = toggle;
    }

    public void SetSpecialIndex(float index)
    {
        activeSpecialIndex = (int)index;
    }

    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            terrainMaterial.EnableKeyword("_SHOW_GRID");
        }
        else
        {
            terrainMaterial.DisableKeyword("_SHOW_GRID");
        }
    }

    public void SetEditMode(bool toggle)
    {
        enabled = toggle;
    }
    #endregion
}
