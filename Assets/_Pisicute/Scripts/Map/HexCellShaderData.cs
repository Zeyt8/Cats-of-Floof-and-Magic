using System.Collections.Generic;
using UnityEngine;

public class HexCellShaderData : MonoBehaviour
{
    private const float TransitionSpeed = 255f;
    
    public bool immediateMode;
    private Texture2D cellTexture;
    private Color32[] cellTextureData;
    private List<HexCell> transitioningCells = new List<HexCell>();
    private bool[] visibilityTransitions;
    private bool needsVisibilityReset;

    private void LateUpdate()
    {
        int delta = (int)(Time.deltaTime * TransitionSpeed);
        if (delta == 0)
        {
            delta = 1;
        }
        for (int i = 0; i < transitioningCells.Count; i++)
        {
            if (!UpdateCellData(transitioningCells[i], delta))
            {
                transitioningCells[i--] = transitioningCells[^1];
                transitioningCells.RemoveAt(transitioningCells.Count - 1);
            }
        }
        cellTexture.SetPixels32(cellTextureData);
        cellTexture.Apply();
        enabled = transitioningCells.Count > 0;
    }
    
    public void Initialize(int x, int z)
    {
        if (cellTexture)
        {
            cellTexture.Reinitialize(x, z);
        }
        else
        {
            cellTexture = new Texture2D(x, z, TextureFormat.RGBA32, false, true);
            cellTexture.filterMode = FilterMode.Point;
            cellTexture.wrapMode = TextureWrapMode.Clamp;
            Shader.SetGlobalTexture("_HexCellData", cellTexture);
        }
        Shader.SetGlobalVector("_HexCellData_TexelSize", new Vector4(1f / x, 1f / z, x, z));
        if (cellTextureData == null || cellTextureData.Length != x * z)
        {
            cellTextureData = new Color32[x * z];
            visibilityTransitions = new bool[x * z];
        }
        else
        {
            for (int i = 0; i < cellTextureData.Length; i++)
            {
                cellTextureData[i] = Color.clear;
                visibilityTransitions[i] = false;
            }
        }
        transitioningCells.Clear();
        enabled = true;
    }

    public void RefreshTerrain(HexCell cell)
    {
        Color32 data = cellTextureData[cell.index];
        data.b = cell.IsUnderwater ? (byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
        data.a = (byte)cell.TerrainTypeIndex;
        cellTextureData[cell.index] = data;
        enabled = true;
    }

    public void RefreshVisibility(HexCell cell)
    {
        int index = cell.index;
        if (immediateMode)
        {
            cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
            cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
        }
        else if (!visibilityTransitions[index])
        {
            visibilityTransitions[index] = true;
            transitioningCells.Add(cell);   
        }
        enabled = true;
    }

    public void SetMapData(HexCell cell, float data)
    {
        cellTextureData[cell.index].b = data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 255f) : (byte)255);
        enabled = true;
    }

    private bool UpdateCellData(HexCell cell, int delta)
    {
        int index = cell.index;
        Color32 data = cellTextureData[index];
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
            visibilityTransitions[index] = false;
        }
        cellTextureData[index] = data;
        return stillUpdating;
    }
}
