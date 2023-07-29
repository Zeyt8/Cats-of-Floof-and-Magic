using System;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    private const int MapFileVersion = 1;

    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private TextMeshProUGUI menuLabel;
    [SerializeField] private TextMeshProUGUI actionButtonLabel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private RectTransform listContent;
    [SerializeField] private SaveLoadItem itemPrefab;

    private bool saveMode;
    public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        if (saveMode)
        {
            menuLabel.text = "Save Map";
            actionButtonLabel.text = "Save";
        }
        else
        {
            menuLabel.text = "Load Map";
            actionButtonLabel.text = "Load";
        }

        FillList();
        gameObject.SetActive(true);
    }

    public void Action()
    {
        string path = GetSelectedPath();
        if (path == null)
        {
            return;
        }
        if (saveMode)
        {
            Save(path);
        }
        else
        {
            Load(path);
        }

        gameObject.SetActive(false);
    }

    public void SelectItem(string name)
    {
        nameInput.text = name;
    }

    public void Delete()
    {
        string path = GetSelectedPath();
        if (path == null)
        {
            return;
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
        FillList();
    }

    private void Save(string path)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create));
        writer.Write(MapFileVersion);
        hexGrid.Save(writer);
    }

    private void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        using BinaryReader reader = new BinaryReader(File.OpenRead(path));
        int header = reader.ReadInt32();
        if (header == MapFileVersion)
        {
            hexGrid.Load(reader, header, hexGrid);
        }
        else
        {
            Debug.LogWarning("Unknown map format " + header);
        }
    }

    private string GetSelectedPath()
    {
        string mapName = nameInput.text;
        return mapName.Length == 0 ? null : Path.Combine(Application.streamingAssetsPath, "Maps", mapName + ".map");
    }

    private void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }
        string[] paths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Maps"), "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab, listContent, false);
            item.menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
        }
    }
}
