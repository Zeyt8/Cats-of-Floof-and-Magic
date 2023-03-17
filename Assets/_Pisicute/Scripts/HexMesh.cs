using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexMesh : MonoBehaviour
{
    [SerializeField] private bool _useCollider;
    [SerializeField] private bool _useCellData;
    [SerializeField] private bool _useUVCoordinates;
    [SerializeField] private bool _useUV2Coordinates;

    private Mesh _hexMesh;
    private MeshCollider _meshCollider;
    [NonSerialized] private List<Vector3> _vertices = new List<Vector3>();
    [NonSerialized] private List<Vector3> _cellIndices = new List<Vector3>();
    [NonSerialized] private List<Color> _cellWeights = new List<Color>();
    [NonSerialized] private List<int> _triangles = new List<int>();
    [NonSerialized] private List<Vector2> _uvs = new List<Vector2>();
    [NonSerialized] private List<Vector2> _uvs2 = new List<Vector2>();

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
        if (_useCollider)
        {
            _meshCollider = GetComponent<MeshCollider>();
        }
        _hexMesh.name = "Hex Mesh";
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(HexMetrics.Perturb(v1));
        _vertices.Add(HexMetrics.Perturb(v2));
        _vertices.Add(HexMetrics.Perturb(v3));
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(HexMetrics.Perturb(v1));
        _vertices.Add(HexMetrics.Perturb(v2));
        _vertices.Add(HexMetrics.Perturb(v3));
        _vertices.Add(HexMetrics.Perturb(v4));
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }

    public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _vertices.Add(v4);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);
        _uvs.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        _uvs.Add(new Vector2(uMin, vMin));
        _uvs.Add(new Vector2(uMax, vMin));
        _uvs.Add(new Vector2(uMin, vMax));
        _uvs.Add(new Vector2(uMax, vMax));
    }

    public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        _uvs2.Add(uv1);
        _uvs2.Add(uv2);
        _uvs2.Add(uv3);
    }

    public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        _uvs2.Add(uv1);
        _uvs2.Add(uv2);
        _uvs2.Add(uv3);
        _uvs2.Add(uv4);
    }

    public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
    {
        _uvs2.Add(new Vector2(uMin, vMin));
        _uvs2.Add(new Vector2(uMax, vMin));
        _uvs2.Add(new Vector2(uMin, vMax));
        _uvs2.Add(new Vector2(uMax, vMax));
    }

    public void AddTriangleCellData(Vector3 indices, Color weights1, Color weights2, Color weights3)
    {
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellWeights.Add(weights1);
        _cellWeights.Add(weights2);
        _cellWeights.Add(weights3);
    }

    public void AddTriangleCellData(Vector3 indices, Color weights)
    {
        AddTriangleCellData(indices, weights, weights, weights);
    }

    public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2, Color weights3, Color weights4)
    {
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellWeights.Add(weights1);
        _cellWeights.Add(weights2);
        _cellWeights.Add(weights3);
        _cellWeights.Add(weights4);
    }

    public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2)
    {
        AddQuadCellData(indices, weights1, weights1, weights2, weights2);
    }

    public void AddQuadCellData(Vector3 indices, Color weights)
    {
        AddQuadCellData(indices, weights, weights, weights, weights);
    }

    public void Clear()
    {
        _hexMesh.Clear();
        _vertices = ListPool<Vector3>.Get();
        if (_useCellData)
        {
            _cellWeights = ListPool<Color>.Get();
            _cellIndices = ListPool<Vector3>.Get();
        }
        if (_useUVCoordinates)
        {
            _uvs = ListPool<Vector2>.Get();
        }
        if (_useUV2Coordinates)
        {
            _uvs2 = ListPool<Vector2>.Get();
        }
        _triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        _hexMesh.SetVertices(_vertices);
        ListPool<Vector3>.Add(_vertices);
        if (_useCellData)
        {
            _hexMesh.SetColors(_cellWeights);
            ListPool<Color>.Add(_cellWeights);
            _hexMesh.SetUVs(2, _cellIndices);
            ListPool<Vector3>.Add(_cellIndices);
        }
        if (_useUVCoordinates)
        {
            _hexMesh.SetUVs(0, _uvs);
            ListPool<Vector2>.Add(_uvs);
        }
        if (_useUV2Coordinates)
        {
            _hexMesh.SetUVs(1, _uvs2);
            ListPool<Vector2>.Add(_uvs2);
        }

        _hexMesh.SetTriangles(_triangles, 0);
        ListPool<int>.Add(_triangles);
        _hexMesh.RecalculateNormals();
        if (_useCollider)
        {
            _meshCollider.sharedMesh = _hexMesh;
        }
    }
}
