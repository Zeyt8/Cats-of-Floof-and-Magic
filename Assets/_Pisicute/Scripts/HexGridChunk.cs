using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HexGridChunk : MonoBehaviour
{
    private static readonly Color Weights1 = new Color(1f, 0f, 0f);
    private static readonly Color Weights2 = new Color(0f, 1f, 0f);
    private static readonly Color Weights3 = new Color(0f, 0f, 1f);
    
    [SerializeField] private HexMesh _terrain;
    [SerializeField] private HexMesh _rivers;
    [SerializeField] private HexMesh _roads;
    [SerializeField] private HexMesh _water;
    [SerializeField] private HexMesh _waterShore;
    [SerializeField] private HexMesh _estuaries;
    [SerializeField] private HexFeatureManager _features;

    private HexCell[] _cells;
    private Canvas _gridCanvas;

    void Awake()
    {
        _gridCanvas = GetComponentInChildren<Canvas>();

        _cells = new HexCell[HexMetrics.ChunkSizeX * HexMetrics.ChunkSizeZ];
    }

    private void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }

    public void AddCell(int index, HexCell cell)
    {
        _cells[index] = cell;
        cell.Chunk = this;
        cell.transform.SetParent(transform, false);
        cell.UiRect.SetParent(_gridCanvas.transform, false);
    }

    public void Refresh()
    {
        enabled = true;
    }

    public void ShowUI(bool visible)
    {
        _gridCanvas.gameObject.SetActive(visible);
    }

    public void Triangulate()
    {
        _terrain.Clear();
        _rivers.Clear();
        _roads.Clear();
        _water.Clear();
        _waterShore.Clear();
        _estuaries.Clear();
        _features.Clear();
        for (int i = 0; i < _cells.Length; i++)
        {
            Triangulate(_cells[i]);
        }
        _terrain.Apply();
        _rivers.Apply();
        _roads.Apply();
        _water.Apply();
        _waterShore.Apply();
        _estuaries.Apply();
        _features.Apply();
    }

    private void Triangulate(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }

        if (!cell.IsUnderwater)
        {
            if (!cell.HasRiver && !cell.HasRoads)
            {
                _features.AddFeature(cell, cell.Position);
            }

            if (cell.IsSpecial)
            {
                _features.AddSpecialFeature(cell, cell.Position);
            }
        }
    }

    private void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.Position;
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );

        if (cell.HasRiver)
        {
            if (cell.HasRiverThroughEdge(direction))
            {
                e.V3.y = cell.StreamBedY;
                if (cell.HasRiverBeginOrEnd)
                {
                    TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
                }
                else
                {
                    TriangulateWithRiver(direction, cell, center, e);
                }
            }
            else
            {
                TriangulateAdjacentToRiver(direction, cell, center, e);
            }
        }
        else
        {
            TriangulateWithoutRiver(direction, cell, center, e);
            if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
            {
                _features.AddFeature(cell, (center + e.V1 + e.V5) * (1f / 3f));
            }
        }

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }

        if (cell.IsUnderwater)
        {
            TriangulateWater(direction, cell, center);
        }
    }

    #region Water
    private void TriangulateWater(HexDirection direction, HexCell cell, Vector3 center)
    {
        center.y = cell.WaterSurfaceY;

        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor != null && !neighbor.IsUnderwater)
        {
            TriangulateWaterShore(direction, cell, neighbor, center);
        }
        else
        {
            TriangulateOpenWater(direction, cell, neighbor, center);
        }
    }

    private void TriangulateOpenWater(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
        Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

        _water.AddTriangle(center, c1, c2);
        Vector3 indices;
        indices.x = indices.y = indices.z = cell.Index;
        _water.AddTriangleCellData(indices, Weights1);

        if (direction <= HexDirection.SE && neighbor != null)
        {
            Vector3 bridge = HexMetrics.GetWaterBridge(direction);
            Vector3 e1 = c1 + bridge;
            Vector3 e2 = c2 + bridge;
            _water.AddQuad(c1, c2, e1, e2);
            indices.y = neighbor.Index;
            _water.AddQuadCellData(indices, Weights1, Weights2);

            if (direction <= HexDirection.E)
            {
                HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
                if (nextNeighbor == null || !nextNeighbor.IsUnderwater)
                {
                    return;
                }
                _water.AddTriangle(c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next()));
                indices.z = nextNeighbor.Index;
                _water.AddTriangleCellData(indices, Weights1, Weights2, Weights3);
            }
        }
    }

    private void TriangulateWaterShore(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.GetFirstWaterCorner(direction),
            center + HexMetrics.GetSecondWaterCorner(direction)
        );
        _water.AddTriangle(center, e1.V1, e1.V2);
        _water.AddTriangle(center, e1.V2, e1.V3);
        _water.AddTriangle(center, e1.V3, e1.V4);
        _water.AddTriangle(center, e1.V4, e1.V5);
        Vector3 indices;
        indices.x = indices.z = cell.Index;
        indices.y = neighbor.Index;
        _water.AddTriangleCellData(indices, Weights1);
        _water.AddTriangleCellData(indices, Weights1);
        _water.AddTriangleCellData(indices, Weights1);
        _water.AddTriangleCellData(indices, Weights1);

        Vector3 center2 = neighbor.Position;
        center2.y = center.y;
        EdgeVertices e2 = new EdgeVertices(
            center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
            center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
        );
        if (cell.HasRiverThroughEdge(direction))
        {
            TriangulateEstuary(e1, e2, cell.HasIncomingRiver && cell.IncomingRiver == direction, indices);
        }
        else
        {
            _waterShore.AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
            _waterShore.AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
            _waterShore.AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
            _waterShore.AddQuad(e1.V4, e1.V5, e2.V4, e2.V5);
            _waterShore.AddQuadUV(0, 0, 0, 1);
            _waterShore.AddQuadUV(0, 0, 0, 1);
            _waterShore.AddQuadUV(0, 0, 0, 1);
            _waterShore.AddQuadUV(0, 0, 0, 1);
            _waterShore.AddQuadCellData(indices, Weights1, Weights2);
            _waterShore.AddQuadCellData(indices, Weights1, Weights2);
            _waterShore.AddQuadCellData(indices, Weights1, Weights2);
            _waterShore.AddQuadCellData(indices, Weights1, Weights2);
        }

        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            Vector3 v3 = nextNeighbor.Position + (nextNeighbor.IsUnderwater ? HexMetrics.GetFirstWaterCorner(direction.Previous()) : HexMetrics.GetFirstSolidCorner(direction.Previous()));
            v3.y = center.y;
            _waterShore.AddTriangle(e1.V5, e2.V5, v3);
            _waterShore.AddTriangleUV(
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(0, nextNeighbor.IsUnderwater ? 0 : 1)
            );
            indices.z = nextNeighbor.Index;
            _waterShore.AddTriangleCellData(indices, Weights1, Weights2, Weights3);
        }
    }

    private void TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float waterY, Vector3 indices)
    {
        v1.y = v2.y = y1;
        v3.y = v4.y = y2;
        v1 = HexMetrics.Perturb(v1);
        v2 = HexMetrics.Perturb(v2);
        v3 = HexMetrics.Perturb(v3);
        v4 = HexMetrics.Perturb(v4);
        float t = (waterY - y2) / (y1 - y2);
        v3 = Vector3.Lerp(v3, v1, t);
        v4 = Vector3.Lerp(v4, v2, t);
        _rivers.AddQuadUnperturbed(v1, v2, v3, v4);
        _rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
        _rivers.AddQuadCellData(indices, Weights1, Weights2);
    }

    private void TriangulateEstuary(EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 indices)
    {
        _waterShore.AddTriangle(e2.V1, e1.V2, e1.V1);
        _waterShore.AddTriangle(e2.V5, e1.V5, e1.V4);
        _waterShore.AddTriangleUV(new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, 0));
        _waterShore.AddTriangleUV(new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, 0));
        _waterShore.AddTriangleCellData(indices, Weights2, Weights1, Weights1);
        _waterShore.AddTriangleCellData(indices, Weights2, Weights1, Weights1);

        _estuaries.AddQuad(e2.V1, e1.V2, e2.V2, e1.V3);
        _estuaries.AddTriangle(e1.V3, e2.V2, e2.V4);
        _estuaries.AddQuad(e1.V3, e1.V4, e2.V4, e2.V5);

        _estuaries.AddQuadUV(new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 0));
        _estuaries.AddTriangleUV(new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 1));
        _estuaries.AddQuadUV(new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1));
        _estuaries.AddQuadCellData(indices, Weights2, Weights1, Weights2, Weights1);
        _estuaries.AddTriangleCellData(indices, Weights1, Weights2, Weights2);
        _estuaries.AddQuadCellData(indices, Weights1, Weights2);

        if (incomingRiver)
        {
            _estuaries.AddQuadUV2(new Vector2(1.5f, 1), new Vector2(0.7f, 1.15f),
                new Vector2(1, 0.8f), new Vector2(0.5f, 1.1f));
            _estuaries.AddTriangleUV2(new Vector2(0.5f, 1.1f), new Vector2(1, 0.8f), new Vector2(0, 0.8f));
            _estuaries.AddQuadUV2(new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
                new Vector2(0, 0.8f), new Vector2(-0.5f, 1));
        }
        else
        {
            _estuaries.AddQuadUV2(
                new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
                new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
            );
            _estuaries.AddTriangleUV2(
                new Vector2(0.5f, -0.3f),
                new Vector2(0f, 0f),
                new Vector2(1f, 0f)
            );
            _estuaries.AddQuadUV2(
                new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
                new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
            );
        }
    }
    #endregion

    #region Rivers
    private void TriangulateAdjacentToRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        if (cell.HasRoads)
        {
            TriangulateRoadAdjacentToRiver(direction, cell, center, e);
        }
        if (cell.HasRiverThroughEdge(direction.Next()))
        {
            if (cell.HasRiverThroughEdge(direction.Previous()))
            {
                center += HexMetrics.GetSolidEdgeMiddle(direction) * (HexMetrics.InnerToOuter * 0.5f);
            }
            else if (cell.HasRiverThroughEdge(direction.Previous2()))
            {
                center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
            }
        }
        else if (cell.HasRiverThroughEdge(direction.Previous()) && cell.HasRiverThroughEdge(direction.Next2()))
        {
            center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
        }
        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(center, e.V1, 0.5f),
            Vector3.Lerp(center, e.V5, 0.5f)
        );

        TriangulateEdgeStrip(m, Weights1, cell.Index, e, Weights1, cell.Index);
        TriangulateEdgeFan(center, m, cell.Index);

        if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
        {
            _features.AddFeature(cell, (center + e.V1 + e.V5) * (1f / 3f));
        }
    }

    private void TriangulateWithRiverBeginOrEnd(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(center, e.V1, 0.5f),
            Vector3.Lerp(center, e.V5, 0.5f)
        );
        m.V3.y = e.V3.y;
        
        TriangulateEdgeStrip(m, Weights1, cell.Index, e, Weights1, cell.Index);
        TriangulateEdgeFan(center, m, cell.Index);

        if (cell.IsUnderwater) return;

        bool reversed = cell.HasIncomingRiver;
        Vector3 indices;
        indices.x = indices.y = indices.z = cell.Index;
        TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, cell.RiverSurfaceY, 0.6f, reversed, indices);

        center.y = m.V2.y = m.V4.y = cell.RiverSurfaceY;
        _rivers.AddTriangle(center, m.V2, m.V4);
        if (reversed)
        {
            _rivers.AddTriangleUV(new Vector2(0.5f, 0.4f), new Vector2(1f, 0.2f), new Vector2(0f, 0.2f));
        }
        else
        {
            _rivers.AddTriangleUV(new Vector2(0.5f, 0.4f), new Vector2(0f, 0.6f), new Vector2(1f, 0.6f));
        }
        _rivers.AddTriangleCellData(indices, Weights1);
    }

    private void TriangulateWithRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        Vector3 centerL;
        Vector3 centerR;
        if (cell.HasRiverThroughEdge(direction.Opposite()))
        {
            centerL = center + HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
            centerR = center + HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
        }
        else if (cell.HasRiverThroughEdge(direction.Next()))
        {
            centerL = center;
            centerR = Vector3.Lerp(center, e.V5, 2f / 3f);
        }
        else if (cell.HasRiverThroughEdge(direction.Previous()))
        {
            centerL = Vector3.Lerp(center, e.V1, 2f / 3f);
            centerR = center;
        }
        else if (cell.HasRiverThroughEdge(direction.Next2()))
        {
            centerL = center;
            centerR = center + HexMetrics.GetSolidEdgeMiddle(direction.Next()) * (0.5f * HexMetrics.InnerToOuter);
        }
        else
        {
            centerL = center + HexMetrics.GetSolidEdgeMiddle(direction.Previous()) * (0.5f * HexMetrics.InnerToOuter);
            centerR = center;
        }
        center = Vector3.Lerp(centerL, centerR, 0.5f);

        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(centerL, e.V1, 0.5f),
            Vector3.Lerp(centerR, e.V5, 0.5f),
            1f / 6f
        );

        m.V3.y = center.y = e.V3.y;

        TriangulateEdgeStrip(m, Weights1, cell.Index, e, Weights1, cell.Index);

        _terrain.AddTriangle(centerL, m.V1, m.V2);
        _terrain.AddQuad(centerL, center, m.V2, m.V3);
        _terrain.AddQuad(center, centerR, m.V3, m.V4);
        _terrain.AddTriangle(centerR, m.V4, m.V5);

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.Index;
        _terrain.AddTriangleCellData(indices, Weights1);
        _terrain.AddQuadCellData(indices, Weights1);
        _terrain.AddQuadCellData(indices, Weights1);
        _terrain.AddTriangleCellData(indices, Weights1);

        if (cell.IsUnderwater) return;
        
        bool reversed = cell.IncomingRiver == direction;
        TriangulateRiverQuad(centerL, centerR, m.V2, m.V4, cell.RiverSurfaceY, 0.4f, reversed, indices);
        TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, cell.RiverSurfaceY, 0.6f, reversed, indices);
    }

    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y, float v, bool reversed, Vector3 indices)
    {
        TriangulateRiverQuad(v1, v2, v3, v4, y, y, v, reversed, indices);
    }

    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float v, bool reversed, Vector3 indices)
    {
        v1.y = v2.y = y1;
        v3.y = v4.y = y2;
        _rivers.AddQuad(v1, v2, v3, v4);
        if (reversed)
        {
            _rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
        }
        else
        {
            _rivers.AddQuadUV(0f, 1f, v, v + 0.2f);
        }
        _rivers.AddQuadCellData(indices, Weights1, Weights2);
    }
    #endregion

    #region Road
    private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6,
        Color w1, Color w2, Vector3 indices)
    {
        _roads.AddQuad(v1, v2, v4, v5);
        _roads.AddQuad(v2, v3, v5, v6);
        _roads.AddQuadUV(0f, 1f, 0f, 0f);
        _roads.AddQuadUV(1f, 0f, 0f, 0f);
        _roads.AddQuadCellData(indices, w1, w2);
        _roads.AddQuadCellData(indices, w1, w2);
    }

    private void TriangulateRoad(Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e, bool hasRoadThroughCellEdge, float index)
    {
        if (hasRoadThroughCellEdge)
        {
            Vector3 indices;
            indices.x = indices.y = indices.z = index;
            Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
            TriangulateRoadSegment(mL, mC, mR, e.V2, e.V3, e.V4, Weights1, Weights1, indices);
            _roads.AddTriangle(center, mL, mC);
            _roads.AddTriangle(center, mC, mR);
            _roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f));
            _roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f));
            _roads.AddTriangleCellData(indices, Weights1);
            _roads.AddTriangleCellData(indices, Weights1);
        }
        else
        {
            TriangulateRoadEdge(center, mL, mR, index);
        }
    }

    private void TriangulateRoadEdge(Vector3 center, Vector3 mL, Vector3 mR, float index)
    {
        _roads.AddTriangle(center, mL, mR);
        _roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        Vector3 indices;
        indices.x = indices.y = indices.z = index;
        _roads.AddTriangleCellData(indices, Weights1);
    }

    private Vector2 GetRoadInterpolators(HexDirection direction, HexCell cell)
    {
        Vector2 interpolators;
        if (cell.HasRoadThroughEdge(direction))
        {
            interpolators.x = interpolators.y = 0.5f;
        }
        else
        {
            interpolators.x = cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
            interpolators.y = cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
        }
        return interpolators;
    }

    private void TriangulateRoadAdjacentToRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
        bool previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
        bool nextHasRiver = cell.HasRiverThroughEdge(direction.Next());
        Vector2 interpolators = GetRoadInterpolators(direction, cell);
        Vector3 roadCenter = center;
        if (cell.HasRiverBeginOrEnd)
        {
            roadCenter += HexMetrics.GetSolidEdgeMiddle(cell.RiverBeginOrEndDirection.Opposite()) * (1f / 3f);
        }
        else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite())
        {
            Vector3 corner;
            if (previousHasRiver)
            {
                if (!hasRoadThroughEdge && !cell.HasRoadThroughEdge(direction.Next()))
                {
                    return;
                }
                corner = HexMetrics.GetSecondSolidCorner(direction);
            }
            else
            {
                if (!hasRoadThroughEdge && !cell.HasRoadThroughEdge(direction.Previous()))
                {
                    return;
                }
                corner = HexMetrics.GetFirstSolidCorner(direction);
            }
            roadCenter += corner * 0.5f;
            if (cell.IncomingRiver == direction.Next() && (cell.HasRoadThroughEdge(direction.Next2()) || cell.HasRoadThroughEdge(direction.Opposite())))
            {
                _features.AddBridge(roadCenter, center - corner * 0.5f);
            }
            center += corner * 0.25f;
        }
        else if (cell.IncomingRiver == cell.OutgoingRiver.Previous())
        {
            roadCenter -= HexMetrics.GetSecondCorner(cell.IncomingRiver) * 0.2f;
        }
        else if (cell.IncomingRiver == cell.OutgoingRiver.Next())
        {
            roadCenter -= HexMetrics.GetFirstCorner(cell.IncomingRiver) * 0.2f;
        }
        else if (previousHasRiver && nextHasRiver)
        {
            if (!hasRoadThroughEdge)
            {
                return;
            }
            Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) * HexMetrics.InnerToOuter;
            roadCenter += offset * 0.7f;
            center += offset * 0.5f;
        }
        else
        {
            HexDirection middle;
            if (previousHasRiver)
            {
                middle = direction.Next();
            }
            else if (nextHasRiver)
            {
                middle = direction.Previous();
            }
            else
            {
                middle = direction;
            }
            if (!cell.HasRoadThroughEdge(middle) && !cell.HasRoadThroughEdge(middle.Previous()) && !cell.HasRoadThroughEdge(middle.Next()))
            {
                return;
            }
            Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
            roadCenter += offset * 0.25f;
            if (direction == middle && cell.HasRoadThroughEdge(direction.Opposite()))
            {
                _features.AddBridge(roadCenter, center - offset * (HexMetrics.InnerToOuter * 0.7f));
            }
        }
        Vector3 mL = Vector3.Lerp(roadCenter, e.V1, interpolators.x);
        Vector3 mR = Vector3.Lerp(roadCenter, e.V5, interpolators.y);
        TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge, cell.Index);

        if (previousHasRiver)
        {
            TriangulateRoadEdge(roadCenter, center, mL, cell.Index);
        }
        if (nextHasRiver)
        {
            TriangulateRoadEdge(roadCenter, mR, center, cell.Index);
        }
    }
    #endregion
    
    private void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }
        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.Position.y - cell.Position.y;
        EdgeVertices e2 = new EdgeVertices(
            e1.V1 + bridge,
            e1.V5 + bridge
        );

        bool hasRiver = cell.HasRiverThroughEdge(direction);
        bool hasRoad = cell.HasRoadThroughEdge(direction);
        
        if (hasRiver)
        {
            e2.V3.y = neighbor.StreamBedY;
            Vector3 indices;
            indices.x = indices.z = cell.Index;
            indices.y = neighbor.Index;
            if (!cell.IsUnderwater)
            {
                if (!neighbor.IsUnderwater)
                {
                    TriangulateRiverQuad(
                        e1.V2, e1.V4, e2.V2, e2.V4,
                        cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
                        cell.HasIncomingRiver && cell.IncomingRiver == direction,
                        indices
                    );
                }
                else if (cell.Elevation > neighbor.WaterLevel)
                {
                    TriangulateWaterfallInWater(
                        e1.V2, e1.V4, e2.V2, e2.V4,
                        cell.RiverSurfaceY, neighbor.RiverSurfaceY, neighbor.WaterSurfaceY,
                        indices
                    );
                }
            }
            else if (!neighbor.IsUnderwater && neighbor.Elevation > cell.WaterLevel)
            {
                TriangulateWaterfallInWater(
                    e2.V4, e2.V2, e1.V4, e1.V2,
                    neighbor.RiverSurfaceY, cell.RiverSurfaceY, cell.WaterSurfaceY,
                    indices
                );
            }
        }

        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
        }
        else
        {
            TriangulateEdgeStrip(e1, Weights1, cell.Index, e2, Weights2, neighbor.Index, hasRoad);
        }

        _features.AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = e1.V5 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Position.y;
            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(e1.V5, cell, e2.V5, neighbor, v5, nextNeighbor);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, e1.V5, cell, e2.V5, neighbor);
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(e2.V5, neighbor, v5, nextNeighbor, e1.V5, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, e1.V5, cell, e2.V5, neighbor);
            }
        }
    }

    private void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell, EdgeVertices end, HexCell endCell, bool hasRoad)
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color w2 = HexMetrics.TerraceLerp(Weights1, Weights2, 1);
        float i1 = beginCell.Index;
        float i2 = endCell.Index;

        TriangulateEdgeStrip(begin, Weights1, i1, e2, w2, i2, hasRoad);

        for (int i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color w1 = w2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            w2 = HexMetrics.TerraceLerp(Weights1, Weights2, i);
            TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad);
        }

        TriangulateEdgeStrip(e2, w2, i1, end, Weights2, i2, hasRoad);
    }

    #region Corners
    private void TriangulateCorner(Vector3 bottom, HexCell bottomCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell)
    {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope)
        {
            switch (rightEdgeType)
            {
                case HexEdgeType.Slope:
                    TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                    break;
                case HexEdgeType.Flat:
                    TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                    break;
                case HexEdgeType.Cliff:
                default:
                    TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
                    break;
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope)
        {
            if (leftEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
            }
            else
            {
                TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            }
        }
        else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            if (leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
            }
            else
            {
                TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
            }
        }
        else
        {
            _terrain.AddTriangle(bottom, left, right);
            Vector3 indices;
            indices.x = bottomCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;
            _terrain.AddTriangleCellData(indices, Weights1, Weights2, Weights3);
        }

        _features.AddWall(bottom, bottomCell, left, leftCell, right, rightCell);
    }

    private void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color w3 = HexMetrics.TerraceLerp(Weights1, Weights2, 1);
        Color w4 = HexMetrics.TerraceLerp(Weights1, Weights3, 1);
        Vector3 indices;
        indices.x = beginCell.TerrainTypeIndex;
        indices.y = leftCell.TerrainTypeIndex;
        indices.z = rightCell.TerrainTypeIndex;

        _terrain.AddTriangle(begin, v3, v4);
        _terrain.AddTriangleCellData(indices, Weights1, w3, w4);

        for (int i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color w1 = w3;
            Color w2 = w4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            w3 = HexMetrics.TerraceLerp(Weights1, Weights2, i);
            w4 = HexMetrics.TerraceLerp(Weights1, Weights3, i);

            _terrain.AddQuad(v1, v2, v3, v4);
            _terrain.AddQuadCellData(indices, w1, w2, w3, w4);
        }

        _terrain.AddQuad(v3, v4, left, right);
        _terrain.AddQuadCellData(indices, w3, w4, Weights2, Weights3);
    }

    private void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell)
    {
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
        Color boundaryWeights = Color.Lerp(Weights1, Weights3, b);
        Vector3 indices;
        indices.x = beginCell.TerrainTypeIndex;
        indices.y = leftCell.TerrainTypeIndex;
        indices.z = rightCell.TerrainTypeIndex;

        TriangulateBoundaryTriangle(begin, Weights1, left, Weights2, boundary, boundaryWeights, indices);
        
        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(left, Weights2, right, Weights3, boundary, boundaryWeights, indices);
        }
        else
        {
            _terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
            _terrain.AddTriangleCellData(indices, Weights2, Weights3, boundaryWeights);
        }
    }

    private void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell)
    {
        float b = 1f / (leftCell.Elevation - beginCell.Elevation);
        b = Mathf.Abs(b);
        Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
        Color boundaryWeights = Color.Lerp(Weights1, Weights2, b);
        Vector3 indices;
        indices.x = beginCell.TerrainTypeIndex;
        indices.y = leftCell.TerrainTypeIndex;
        indices.z = rightCell.TerrainTypeIndex;

        TriangulateBoundaryTriangle(right, Weights3, begin, Weights1, boundary, boundaryWeights, indices);

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(left, Weights2, right, Weights3, boundary, boundaryWeights, indices);
        }
        else
        {
            _terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
            _terrain.AddTriangleCellData(indices, Weights2, Weights3, boundaryWeights);
        }
    }
    
    private void TriangulateBoundaryTriangle(Vector3 begin, Color beginWeights,
        Vector3 left, Color leftWeights,
        Vector3 boundary, Color boundaryWeights, Vector3 indices)
    {
        Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        Color w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);

        _terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
        _terrain.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);

        for (int i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color w1 = w2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, i);
            _terrain.AddTriangleUnperturbed(v1, v2, boundary);
            _terrain.AddTriangleCellData(indices, w1, w2, boundaryWeights);
        }

        _terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
        _terrain.AddTriangleCellData(indices, w2, leftWeights, boundaryWeights);
    }
    #endregion

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)
    {
        _terrain.AddTriangle(center, edge.V1, edge.V2);
        _terrain.AddTriangle(center, edge.V2, edge.V3);
        _terrain.AddTriangle(center, edge.V3, edge.V4);
        _terrain.AddTriangle(center, edge.V4, edge.V5);

        Vector3 indices;
        indices.x = indices.y = indices.z = index;
        _terrain.AddTriangleCellData(indices, Weights1);
        _terrain.AddTriangleCellData(indices, Weights1);
        _terrain.AddTriangleCellData(indices, Weights1);
        _terrain.AddTriangleCellData(indices, Weights1);
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color w1, float index1, EdgeVertices e2, Color w2, float index2, bool hasRoad = false)
    {
        _terrain.AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
        _terrain.AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
        _terrain.AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
        _terrain.AddQuad(e1.V4, e1.V5, e2.V4, e2.V5);

        Vector3 indices;
        indices.x = indices.z = index1;
        indices.y = index2;
        _terrain.AddQuadCellData(indices, w1, w2);
        _terrain.AddQuadCellData(indices, w1, w2);
        _terrain.AddQuadCellData(indices, w1, w2);
        _terrain.AddQuadCellData(indices, w1, w2);

        if (hasRoad)
        {
            TriangulateRoadSegment(e1.V2, e1.V3, e1.V4, e2.V2, e2.V3, e2.V4, w1, w2, indices);
        }
    }

    private void TriangulateWithoutRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        TriangulateEdgeFan(center, e, cell.Index);

        if (cell.HasRoads)
        {
            Vector2 interpolators = GetRoadInterpolators(direction, cell);
            TriangulateRoad(center, Vector3.Lerp(center, e.V1, interpolators.x), Vector3.Lerp(center, e.V5, interpolators.y), e, cell.HasRoadThroughEdge(direction), cell.Index);
        }
    }
}
