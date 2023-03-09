using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.IO;

public class MapEditor : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private HexGrid _hexGrid;
    private int _activeTerrainTypeIndex;
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

    private void Update()
    {
        if (_inputHandler.MapEditor.IsEditing && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else
        {
            _previousCell = null;
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(inputRay.origin, inputRay.direction * 100, Color.red);
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            HexCell currentCell = _hexGrid.GetCell(hit.point);
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
        if (_walledMode != OptionalToggle.Ignore)
        {
            cell.Walled = _walledMode == OptionalToggle.Yes;
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

    public void ShowUI(bool visible)
    {
        _hexGrid.ShowUI(visible);
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
    #endregion
}
