using System;
using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
    [SerializeField] private HexMesh _walls;
    [SerializeField] private HexFeatureCollection[] _urbanCollections, _farmCollections, _plantCollections;
    [SerializeField] private GameObject[] _special;
    [SerializeField] private GameObject _wallTower;
    [SerializeField] private GameObject _bridge;
    private Transform _container;
    
    public void Clear()
    {
        if (_container)
        {
            Destroy(_container.gameObject);
        }
        _container = new GameObject("Features Container").transform;
        _container.SetParent(transform, false);
        _walls.Clear();
    }

    public void Apply()
    {
        _walls.Apply();
    }

    public void AddFeature(HexCell cell, Vector3 position)
    {
        if (cell.IsSpecial) return;
        
        HexHash hash = HexMetrics.SampleHashGrid(position);
        GameObject prefab = PickPrefab(_urbanCollections, cell.UrbanLevel, hash.A, hash.D);
        GameObject otherPrefab = PickPrefab(_farmCollections, cell.FarmLevel, hash.B, hash.D);
        float usedHash = hash.A;
        if (prefab)
        {
            if (otherPrefab && hash.B < hash.A)
            {
                prefab = otherPrefab;
                usedHash = hash.B;
            }
        }
        else if (otherPrefab)
        {
            prefab = otherPrefab;
            usedHash = hash.B;
        }
        otherPrefab = PickPrefab(_plantCollections, cell.PlantLevel, hash.C, hash.D);
        if (prefab)
        {
            if (otherPrefab && hash.C < usedHash)
            {
                prefab = otherPrefab;
            }
        }
        else if (otherPrefab)
        {
            prefab = otherPrefab;
        }
        else
        {
            return;
        }
        GameObject instance = Instantiate(prefab, _container, false);
        instance.transform.localPosition = HexMetrics.Perturb(position);
        instance.transform.localRotation = Quaternion.Euler(0f, 360f * hash.E, 0f);
    }

    public void AddWall(EdgeVertices near, HexCell nearCell, EdgeVertices far, HexCell farCell, bool hasRiver, bool hasRoad)
    {
        if (!nearCell.HasWallThroughEdge(nearCell.GetNeighborDirection(farCell))) return;
        AddWallSegment(near.V1, far.V1, near.V2, far.V2);
        if (hasRiver || hasRoad)
        {
            AddWallCap(near.V2, far.V2);
            AddWallCap(far.V4, near.V4);
        }
        else
        {
            AddWallSegment(near.V2, far.V2, near.V3, far.V3);
            AddWallSegment(near.V3, far.V3, near.V4, far.V4);
        }
        AddWallSegment(near.V4, far.V4, near.V5, far.V5);
    }

    public void AddWall(Vector3 c1, HexCell cell1, Vector3 c2, HexCell cell2, Vector3 c3, HexCell cell3)
    {
        // All have walls
        if (cell1.HasWallThroughEdge(cell1.GetNeighborDirection(cell2)) &&
            cell2.HasWallThroughEdge(cell2.GetNeighborDirection(cell3)) &&
            cell3.HasWallThroughEdge(cell3.GetNeighborDirection(cell1)))
        {
            //TODO: Add wall
            print("all");
        }
        else if (cell1.HasWallThroughEdge(cell1.GetNeighborDirection(cell2)) && cell1.HasWallThroughEdge(cell1.GetNeighborDirection(cell3)))
        {
            AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
        }
        else if (cell2.HasWallThroughEdge(cell2.GetNeighborDirection(cell1)) && cell2.HasWallThroughEdge(cell2.GetNeighborDirection(cell3)))
        {
            AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
        }
        else if (cell3.HasWallThroughEdge(cell3.GetNeighborDirection(cell1)) && cell3.HasWallThroughEdge(cell3.GetNeighborDirection(cell2)))
        {
            AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
        }
        else if (cell1.HasWallThroughEdge(cell1.GetNeighborDirection(cell2)))
        {
            AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
            AddWallSegment(c1, cell1, c3, cell3, c2, cell2);
        }
        else if (cell2.HasWallThroughEdge(cell2.GetNeighborDirection(cell3)))
        {
            AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
            AddWallSegment(c2, cell2, c1, cell1, c3, cell3);
        }
        else if (cell3.HasWallThroughEdge(cell3.GetNeighborDirection(cell1)))
        {
            AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
            AddWallSegment(c3, cell3, c2, cell2, c1, cell1);
        }
    }

    private GameObject PickPrefab(HexFeatureCollection[] collection, int level, float hash, float choice)
    {
        if (level > 0)
        {
            float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (hash < thresholds[i])
                {
                    return collection[i].Pick(choice);
                }
            }
        }
        return null;
    }

    private void AddWallSegment(Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight, bool addTower = false)
    {
        nearLeft = HexMetrics.Perturb(nearLeft);
        farLeft = HexMetrics.Perturb(farLeft);
        nearRight = HexMetrics.Perturb(nearRight);
        farRight = HexMetrics.Perturb(farRight);
        
        Vector3 left = HexMetrics.WallLerp(nearLeft, farLeft);
        Vector3 right = HexMetrics.WallLerp(nearRight, farRight);
        Vector3 leftThicknessOffset = HexMetrics.WallThicknessOffset(nearLeft, farLeft);
        Vector3 rightThicknessOffset = HexMetrics.WallThicknessOffset(nearRight, farRight);
        float leftTop = left.y + HexMetrics.WallHeight;
        float rightTop = right.y + HexMetrics.WallHeight;

        Vector3 v1, v2, v3, v4;
        v1 = v3 = left - leftThicknessOffset;
        v2 = v4 = right - rightThicknessOffset;
        v3.y = leftTop;
        v4.y = rightTop;
        _walls.AddQuadUnperturbed(v1, v2, v3, v4);

        Vector3 t1 = v3, t2 = v4;

        v1 = v3 = left + leftThicknessOffset;
        v2 = v4 = right + rightThicknessOffset;
        v3.y = leftTop;
        v4.y = rightTop;
        _walls.AddQuadUnperturbed(v2, v1, v4, v3);

        _walls.AddQuadUnperturbed(t1, t2, v3, v4);

        if (addTower)
        {
            GameObject towerInstance = Instantiate(_wallTower, _container, false);
            towerInstance.transform.localPosition = (left + right) * 0.5f;
            Vector3 rightDirection = right - left;
            towerInstance.transform.right = rightDirection;
        }
    }

    private void AddWallSegment(Vector3 pivot, HexCell pivotCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        if (pivotCell.IsUnderwater) return;
        bool hasLeftWall = pivotCell.HasWallThroughEdge(pivotCell.GetNeighborDirection(leftCell));
        bool hasRightWall = pivotCell.HasWallThroughEdge(pivotCell.GetNeighborDirection(rightCell));
        
        if (hasLeftWall)
        {
            if (hasRightWall)
            {
                bool hasTower = false;
                if (leftCell.Elevation == rightCell.Elevation)
                {
                    HexHash hash = HexMetrics.SampleHashGrid((pivot + left + right) / 3);
                    hasTower = hash.E < HexMetrics.WallTowerThreshold;
                }
                AddWallSegment(pivot, left, pivot, right, hasTower);
            }
            else if (leftCell.Elevation < rightCell.Elevation)
            {
                AddWallWedge(pivot, left, right);
            }
            else
            {
                AddWallCap(pivot, left);
            }
        }
        else if (hasRightWall)
        {
            if (rightCell.Elevation < leftCell.Elevation)
            {
                AddWallWedge(right, pivot, left);
            }
            else
            {
                AddWallCap(right, pivot);
            }
        }
    }

    private void AddWallCap(Vector3 near, Vector3 far)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);

        Vector3 center = HexMetrics.WallLerp(near, far);
        Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

        Vector3 v1, v2, v3, v4;

        v1 = v3 = center - thickness;
        v2 = v4 = center + thickness;
        v3.y = v4.y = center.y + HexMetrics.WallHeight;
        _walls.AddQuadUnperturbed(v1, v2, v3, v4);
    }

    private void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);
        point = HexMetrics.Perturb(point);

        Vector3 center = HexMetrics.WallLerp(near, far);
        Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

        Vector3 v1, v2, v3, v4;
        Vector3 pointTop = point;
        point.y = center.y;

        v1 = v3 = center - thickness;
        v2 = v4 = center + thickness;
        v3.y = v4.y = pointTop.y = center.y + HexMetrics.WallHeight;

        _walls.AddQuadUnperturbed(v1, point, v3, pointTop);
        _walls.AddQuadUnperturbed(point, v2, pointTop, v4);
        _walls.AddTriangleUnperturbed(pointTop, v3, v4);
    }

    public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
    {
        roadCenter1 = HexMetrics.Perturb(roadCenter1);
        roadCenter2 = HexMetrics.Perturb(roadCenter2);
        GameObject instance = Instantiate(_bridge, _container, false);
        instance.transform.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
        instance.transform.forward = roadCenter2 - roadCenter1;
        float length = Vector3.Distance(roadCenter1, roadCenter2);
        instance.transform.localScale = new Vector3(1f, 1f, length * (1f / HexMetrics.BridgeDesignLength));
    }

    public void AddSpecialFeature(HexCell cell, Vector3 position)
    {
        GameObject instance = Instantiate(_special[cell.SpecialIndex - 1], _container, false);
        instance.transform.localPosition = HexMetrics.Perturb(position);
        HexHash hash = HexMetrics.SampleHashGrid(position);
        instance.transform.localRotation = Quaternion.Euler(0f, 360f * hash.E, 0f);
    }
}
