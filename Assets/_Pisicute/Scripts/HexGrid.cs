using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int Width = 6;
    public int Height = 6;
    public Color DefaultColor = Color.white;
    public Color SelectedColor = Color.cyan;
    public HexCell CellPrefab;
    public TextMeshProUGUI CellLabelPrefab;

    private HexCell[] _cells;
    private Canvas _gridCanvas;
    private HexMesh _hexMesh;
    
    private void Awake()
    {
        _gridCanvas = GetComponentInChildren<Canvas>();
        _hexMesh = GetComponentInChildren<HexMesh>();
        _cells = new HexCell[Width * Height];
        for (int z = 0, i = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void Start()
    {
        _hexMesh.Triangulate(_cells);
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.OuterRadius * 1.5f);

        HexCell cell = _cells[i] = Instantiate(CellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.Coordinates = new HexCoordinates(x, z);
        cell.Color = DefaultColor;
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
        }
        if (z > 0)
        {
            if (z % 2 == 0)
            {
                cell.SetNeighbor(HexDirection.SE, _cells[i - Width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - Width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, _cells[i - Width]);
                if (x < Width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - Width + 1]);
                }
            }
        }

        // UI labels
        TextMeshProUGUI label = Instantiate(CellLabelPrefab);
        label.rectTransform.SetParent(_gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.Coordinates.ToStringOnSeparateLines();

        cell.UiRect = label.rectTransform;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * Width + coordinates.Z / 2;
        return _cells[index];
    }

    public void Refresh()
    {
        _hexMesh.Triangulate(_cells);
    }
}
