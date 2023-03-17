using System;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    private const int MapFileVersion = 1;

    [SerializeField] private HexGrid _hexGrid;
    [SerializeField] private TextMeshProUGUI _menuLabel;
    [SerializeField] private TextMeshProUGUI _actionButtonLabel;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private RectTransform _listContent;
    [SerializeField] private SaveLoadItem _itemPrefab;

    private bool _saveMode;
    public void Open(bool saveMode)
    {
        _saveMode = saveMode;
        if (saveMode)
        {
            _menuLabel.text = "Save Map";
            _actionButtonLabel.text = "Save";
        }
        else
        {
            _menuLabel.text = "Load Map";
            _actionButtonLabel.text = "Load";
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
        if (_saveMode)
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
        _nameInput.text = name;
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
        _nameInput.text = "";
        FillList();
    }

    private void Save(string path)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create));
        writer.Write(MapFileVersion);
        _hexGrid.Save(writer);
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
            _hexGrid.Load(reader, header);
        }
        else
        {
            Debug.LogWarning("Unknown map format " + header);
        }
    }

    private string GetSelectedPath()
    {
        string mapName = _nameInput.text;
        return mapName.Length == 0 ? null : Path.Combine(Application.persistentDataPath, mapName + ".map");
    }

    private void FillList()
    {
        for (int i = 0; i < _listContent.childCount; i++)
        {
            Destroy(_listContent.GetChild(i).gameObject);
        }
        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(_itemPrefab, _listContent, false);
            item.Menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
        }
    }
}
