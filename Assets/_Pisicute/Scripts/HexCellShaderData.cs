using System.Collections.Generic;
using UnityEngine;

public class HexCellShaderData : MonoBehaviour
{
    private const float TransitionSpeed = 255f;
    
    public bool ImmediateMode;
    private Texture2D _cellTexture;
    private Color32[] _cellTextureData;
    private List<HexCell> _transitioningCells = new List<HexCell>();
    private bool[] _visibilityTransitions;
    private bool _needsVisibilityReset;

    private void LateUpdate()
    {
        int delta = (int)(Time.deltaTime * TransitionSpeed);
        if (delta == 0)
        {
            delta = 1;
        }
        for (int i = 0; i < _transitioningCells.Count; i++)
        {
            if (!UpdateCellData(_transitioningCells[i], delta))
            {
                _transitioningCells[i--] = _transitioningCells[^1];
                _transitioningCells.RemoveAt(_transitioningCells.Count - 1);
            }
        }
        _cellTexture.SetPixels32(_cellTextureData);
        _cellTexture.Apply();
        enabled = _transitioningCells.Count > 0;
    }
    
    public void Initialize(int x, int z)
    {
        if (_cellTexture)
        {
            _cellTexture.Reinitialize(x, z);
        }
        else
        {
            _cellTexture = new Texture2D(x, z, TextureFormat.RGBA32, false, true);
            _cellTexture.filterMode = FilterMode.Point;
            _cellTexture.wrapMode = TextureWrapMode.Clamp;
            Shader.SetGlobalTexture("_HexCellData", _cellTexture);
        }
        Shader.SetGlobalVector("_HexCellData_TexelSize", new Vector4(1f / x, 1f / z, x, z));
        if (_cellTextureData == null || _cellTextureData.Length != x * z)
        {
            _cellTextureData = new Color32[x * z];
            _visibilityTransitions = new bool[x * z];
        }
        else
        {
            for (int i = 0; i < _cellTextureData.Length; i++)
            {
                _cellTextureData[i] = Color.clear;
                _visibilityTransitions[i] = false;
            }
        }
        _transitioningCells.Clear();
        enabled = true;
    }

    public void RefreshTerrain(HexCell cell)
    {
        Color32 data = _cellTextureData[cell.Index];
        data.b = cell.IsUnderwater ? (byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
        data.a = (byte)cell.TerrainTypeIndex;
        _cellTextureData[cell.Index] = data;
        enabled = true;
    }

    public void RefreshVisibility(HexCell cell)
    {
        int index = cell.Index;
        if (ImmediateMode)
        {
            _cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
            _cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
        }
        else if (!_visibilityTransitions[index])
        {
            _visibilityTransitions[index] = true;
            _transitioningCells.Add(cell);   
        }
        enabled = true;
    }

    public void SetMapData(HexCell cell, float data)
    {
        _cellTextureData[cell.Index].b = data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 255f) : (byte)255);
        enabled = true;
    }

    private bool UpdateCellData(HexCell cell, int delta)
    {
        int index = cell.Index;
        Color32 data = _cellTextureData[index];
        bool stillUpdating = false;
        if (cell.IsExplored && data.g < 255)
        {
            stillUpdating = true;
            int t = data.g + delta;
            data.g = t >= 255 ? (byte)255 : (byte)t;
        }
        if (cell.IsVisible)
        {
            if (data.r < 255)
            {
                stillUpdating = true;
                int t = data.r + delta;
                data.r = t >= 255 ? (byte)255 : (byte)t;
            }
        }
        else if (data.r > 0)
        {
            stillUpdating = true;
            int t = data.r - delta;
            data.r = t < 0 ? (byte)0 : (byte)t;
        }
        if (!stillUpdating)
        {
            _visibilityTransitions[index] = false;
        }
        _cellTextureData[index] = data;
        return stillUpdating;
    }
}
