using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapEditor : MonoBehaviour
{
    static int cellHighlightingId = Shader.PropertyToID("_CellHighlighting");
    
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private HexGrid _hexGrid;
    [SerializeField] private Material _terrainMaterial;
    private int _activeTerrainTypeIndex = -1;
    private int _activeElevation;
    private int _activeWaterLevel;
    private int _activeUrbanLevel, _activeFarmLevel, _activePlantLevel, _activeSpecialIndex;
    private bool _applyElevation;
    private bool _applyWaterLevel;
    private bool _applyUrbanLevel, _applyFarmLevel, _applyPlantLevel, _applySpecialIndex;
    private int _brushSize;
    private OptionalToggle _riverMode;
    private OptionalToggle _roadMode;
    private OptionalToggle _walledMode;
    private bool _isDrag;
    private HexDirection _dragDirection;
    private HexCell _previousCell;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    private void Awake()
    {
        _terrainMaterial.DisableKeyword("_SHOW_GRID");
        Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
        SetEditMode(true);
    }

    private void OnEnable()
    {
        _inputHandler.MapEditor.OnCreateUnit.AddListener(CreateUnit);
        _inputHandler.MapEditor.OnDestroyUnit.AddListener(DestroyUnit);
    }

    private void OnDisable()
    {
        _inputHandler.MapEditor.OnCreateUnit.RemoveListener(CreateUnit);
        _inputHandler.MapEditor.OnDestroyUnit.RemoveListener(DestroyUnit);
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (_inputHandler.MapEditor.IsEditing)
            {
                HandleInput();
            }
            else
            {
                _previousCell = null;
            }
            UpdateCellHighlightData(GetCellUnderCursor());
        }
        else
        {
            _previousCell = null;
            ClearCellHighlightData();
        }
    }

    private void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
            if (_previousCell && _previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                _isDrag = false;
            }
            EditCells(currentCell);
            _previousCell = currentCell;
        }
        else
        {
            _previousCell = null;
        }
    }

    private void EditCell(HexCell cell)
    {
        if (!cell) return;
        if (_activeTerrainTypeIndex >= 0)
        {
            cell.TerrainTypeIndex = _activeTerrainTypeIndex;
        }
        if (_applyElevation)
        {
            cell.Elevation = _activeElevation;
        }
        if (_applyWaterLevel)
        {
            cell.WaterLevel = _activeWaterLevel;
        }
        if (_applyUrbanLevel)
        {
            cell.UrbanLevel = _activeUrbanLevel;
        }
        if (_applyFarmLevel)
        {
            cell.FarmLevel = _activeFarmLevel;
        }
        if (_applyPlantLevel)
        {
            cell.PlantLevel = _activePlantLevel;
        }
        if (_applySpecialIndex)
        {
            cell.SpecialIndex = _activeSpecialIndex;
        }
        if (_riverMode == OptionalToggle.No)
        {
            cell.RemoveRiver();
        }
        if (_roadMode == OptionalToggle.No)
        {
            cell.RemoveRoads();
        }
        if (_walledMode == OptionalToggle.No)
        {
            cell.RemoveWall();
        }
        if (_isDrag)
        {
            HexCell otherCell = cell.GetNeighbor(_dragDirection.Opposite());
            if (otherCell)
            {
                if (_riverMode == OptionalToggle.Yes)
                {
                    otherCell.SetOutgoingRiver(_dragDirection);
                }
                if (_roadMode == OptionalToggle.Yes)
                {
                    otherCell.AddRoad(_dragDirection);
                }
                if (_walledMode == OptionalToggle.Yes)
                {
                    otherCell.AddWall(_dragDirection);
                }
            }
        }
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.Coordinates.X;
        int centerZ = center.Coordinates.Z;
        for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + _brushSize; x++)
            {
                EditCell(_hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - _brushSize; x <= centerX + r; x++)
            {
                EditCell(_hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private void ValidateDrag(HexCell currentCell)
    {
        for (_dragDirection = HexDirection.NE; _dragDirection <= HexDirection.NW; _dragDirection++)
        {
            if (_previousCell.GetNeighbor(_dragDirection) == currentCell)
            {
                _isDrag = true;
                return;
            }
        }
        _isDrag = false;
    }
    private HexCell GetCellUnderCursor()
    {
        return _hexGrid.GetCell(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
    }

    private void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.Unit)
        {
            _hexGrid.AddUnit(Instantiate(_hexGrid.UnitPrefab), cell, Random.Range(0, 360f));
        }
    }

    private void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.Unit)
        {
            _hexGrid.RemoveUnit(cell.Unit);
        }
    }

    void UpdateCellHighlightData(HexCell cell)
    {
        if (cell == null)
        {
            ClearCellHighlightData();
            return;
        }

        Shader.SetGlobalVector(cellHighlightingId, new Vector4(cell.Coordinates.HexX, cell.Coordinates.HexZ, _brushSize * _brushSize + 0.5f, 0));
    }

    void ClearCellHighlightData()
    {
        Shader.SetGlobalVector(cellHighlightingId, new Vector4(0f, 0f, -1f, 0f));
    }

    #region UI
    public void SetTerrainTypeIndex(int index)
    {
        _activeTerrainTypeIndex = index;
    }

    public void SetElevation(float elevation)
    {
        _activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        _applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        _brushSize = (int)size;
    }

    public void SetRiverMode(int mode)
    {
        _riverMode = (OptionalToggle)mode;
    }

    public void SetRoadMode(int mode)
    {
        _roadMode = (OptionalToggle)mode;
    }

    public void SetApplyWaterLevel(bool toggle)
    {
        _applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        _activeWaterLevel = (int)level;
    }

    public void SetApplyUrbanLevel(bool toggle)
    {
        _applyUrbanLevel = toggle;
    }

    public void SetUrbanLevel(float level)
    {
        _activeUrbanLevel = (int)level;
    }

    public void SetApplyFarmLevel(bool toggle)
    {
        _applyFarmLevel = toggle;
    }

    public void SetFarmLevel(float level)
    {
        _activeFarmLevel = (int)level;
    }

    public void SetApplyPlantLevel(bool toggle)
    {
        _applyPlantLevel = toggle;
    }

    public void SetPlantLevel(float level)
    {
        _activePlantLevel = (int)level;
    }

    public void SetWalledMode(int mode)
    {
        _walledMode = (OptionalToggle)mode;
    }

    public void SetApplySpecialIndex(bool toggle)
    {
        _applySpecialIndex = toggle;
    }

    public void SetSpecialIndex(float index)
    {
        _activeSpecialIndex = (int)index;
    }

    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            _terrainMaterial.EnableKeyword("_SHOW_GRID");
        }
        else
        {
            _terrainMaterial.DisableKeyword("_SHOW_GRID");
        }
    }

    public void SetEditMode(bool toggle)
    {
        enabled = toggle;
    }
    #endregion
}
