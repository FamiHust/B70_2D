using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private ItemsCollection _itemsCollection;
    private ShopLayoutData _shopLayoutData;
    private Vector2 _scrollPos;
    private int _selectedItemIndex = 0;
    private string[] _itemNames;

    private const int GridWidth = 60;
    private const int GridHeight = 60;
    private const float CellSize = 30f;

    private int[,] _grid; // Stores index of ShopLayoutItem in _shopLayoutData.items, or -1

    [MenuItem("Window/Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("Map Editor");
    }

    private void OnEnable()
    {
        LoadItemsCollection();
        LoadData();
    }

    private void LoadItemsCollection()
    {
        _itemsCollection = Resources.Load<ItemsCollection>("ItemsCollection");
        if (_itemsCollection != null)
        {
            _itemNames = new string[_itemsCollection.list.Count];
            for (int i = 0; i < _itemsCollection.list.Count; i++)
            {
                _itemNames[i] = _itemsCollection.list[i].name + " (" + _itemsCollection.list[i].gridWidth + "x" + _itemsCollection.list[i].gridHeight + ")";
            }
        }
    }

    private void LoadData()
    {
        string filePath = Application.dataPath + "/StreamingAssets/shop_layout.json";
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            _shopLayoutData = JsonUtility.FromJson<ShopLayoutData>(jsonData);
        }
        
        if (_shopLayoutData == null) _shopLayoutData = new ShopLayoutData();
        if (_shopLayoutData.items == null) _shopLayoutData.items = new List<ShopLayoutItem>();
        
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        _grid = new int[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                _grid[x, z] = -1;
            }
        }

        for (int i = 0; i < _shopLayoutData.items.Count; i++)
        {
            ShopLayoutItem item = _shopLayoutData.items[i];
            ItemsCollection.ItemData config = GetItemConfig(item.itemId);
            if (config != null)
            {
                for (int dx = 0; dx < config.gridWidth; dx++)
                {
                    for (int dz = 0; dz < config.gridHeight; dz++)
                    {
                        int gx = item.posX + dx;
                        int gz = item.posZ + dz;
                        if (gx >= 0 && gx < GridWidth && gz >= 0 && gz < GridHeight)
                        {
                            _grid[gx, gz] = i;
                        }
                    }
                }
            }
        }
    }

    private ItemsCollection.ItemData GetItemConfig(int id)
    {
        foreach (var item in _itemsCollection.list)
        {
            if (item.id == id) return item;
        }
        return null;
    }

    private void SaveData()
    {
        string filePath = Application.dataPath + "/StreamingAssets/shop_layout.json";
        string jsonData = JsonUtility.ToJson(_shopLayoutData);
        File.WriteAllText(filePath, jsonData);
        AssetDatabase.Refresh();
        Debug.Log("Shop Layout saved to " + filePath);
    }

    private void OnGUI()
    {
        if (_itemsCollection == null)
        {
            EditorGUILayout.HelpBox("ItemsCollection not found in Resources!", MessageType.Error);
            if (GUILayout.Button("Retry Load")) LoadItemsCollection();
            return;
        }

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Load", EditorStyles.toolbarButton)) LoadData();
        if (GUILayout.Button("Save", EditorStyles.toolbarButton)) SaveData();
        if (GUILayout.Button("Clear All", EditorStyles.toolbarButton)) {
            if (EditorUtility.DisplayDialog("Clear Layout", "Are you sure you want to delete all positions?", "Yes", "No")) {
                _shopLayoutData.items.Clear();
                UpdateGrid();
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        
        // Left Panel: Selection
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        EditorGUILayout.LabelField("Building Selection", EditorStyles.boldLabel);
        _selectedItemIndex = EditorGUILayout.Popup("Building", _selectedItemIndex, _itemNames);
        
        if (_selectedItemIndex >= 0 && _selectedItemIndex < _itemsCollection.list.Count)
        {
            var selectedItem = _itemsCollection.list[_selectedItemIndex];
            EditorGUILayout.LabelField("Size: " + selectedItem.gridWidth + "x" + selectedItem.gridHeight);
            if (selectedItem.thumb != null)
            {
                GUILayout.Label(selectedItem.thumb, GUILayout.Width(100), GUILayout.Height(100));
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("LEFT CLICK: Set/Move Position\nRIGHT CLICK: Clear Position", MessageType.Info);
        EditorGUILayout.HelpBox("Note: Each building type has only one fixed position.", MessageType.Warning);
        
        EditorGUILayout.EndVertical();

        // Right Panel: Grid
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        const float LabelWidth = 30f;
        Rect containerRect = GUILayoutUtility.GetRect((GridWidth * CellSize) + LabelWidth, (GridHeight * CellSize) + LabelWidth);
        DrawGrid(containerRect, labelWidth: LabelWidth);
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGrid(Rect rect, float labelWidth)
    {
        Event e = Event.current;
        
        // Draw Column Indices (Top)
        for (int x = 0; x < GridWidth; x++)
        {
            Rect labelRect = new Rect(rect.x + labelWidth + x * CellSize, rect.y, CellSize, labelWidth);
            GUI.Label(labelRect, x.ToString(), GetHeaderStyle());
        }

        // Draw Row Indices (Left)
        for (int z = 0; z < GridHeight; z++)
        {
            Rect labelRect = new Rect(rect.x, rect.y + labelWidth + (GridHeight - 1 - z) * CellSize, labelWidth, CellSize);
            GUI.Label(labelRect, z.ToString(), GetHeaderStyle());
        }

        Rect gridContentRect = new Rect(rect.x + labelWidth, rect.y + labelWidth, GridWidth * CellSize, GridHeight * CellSize);
        GUI.BeginGroup(gridContentRect);

        // Draw Background
        EditorGUI.DrawRect(new Rect(0, 0, gridContentRect.width, gridContentRect.height), new Color(0.15f, 0.15f, 0.15f));

        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                Rect cellRect = new Rect(x * CellSize, (GridHeight - 1 - z) * CellSize, CellSize, CellSize);
                
                // Draw Cell Border
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), new Color(0.3f, 0.3f, 0.3f));
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), new Color(0.3f, 0.3f, 0.3f));

                int itemIndex = _grid[x, z];
                if (itemIndex != -1)
                {
                    ShopLayoutItem item = _shopLayoutData.items[itemIndex];
                    bool isAnchor = (item.posX == x && item.posZ == z);
                    
                    Color cellColor = isAnchor ? new Color(0.2f, 0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.6f, 0.6f, 0.4f); // Cyan for Layout
                    EditorGUI.DrawRect(new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.width - 1, cellRect.height - 1), cellColor);
                    
                    if (isAnchor)
                    {
                        GUI.Label(cellRect, item.itemId.ToString(), GetLabelStyle());
                    }
                }

                // Interaction
                if (cellRect.Contains(e.mousePosition))
                {
                    // Hover highlight
                    EditorGUI.DrawRect(new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.width - 2, cellRect.height - 2), new Color(1, 1, 1, 0.1f));

                    if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                    {
                        if (e.button == 0) // Left Click -> Set Position
                        {
                            SetItemPosition(x, z);
                            e.Use();
                        }
                        else if (e.button == 1) // Right Click -> Remove
                        {
                            RemoveItemAt(x, z);
                            e.Use();
                        }
                    }
                }
            }
        }

        // Draw outer borders
        EditorGUI.DrawRect(new Rect(0, GridHeight * CellSize, GridWidth * CellSize, 1), new Color(0.3f, 0.3f, 0.3f));
        EditorGUI.DrawRect(new Rect(GridWidth * CellSize, 0, 1, GridHeight * CellSize), new Color(0.3f, 0.3f, 0.3f));

        GUI.EndGroup();
    }

    private GUIStyle _headerStyle;
    private GUIStyle GetHeaderStyle()
    {
        if (_headerStyle == null)
        {
            _headerStyle = new GUIStyle(EditorStyles.miniLabel);
            _headerStyle.alignment = TextAnchor.MiddleCenter;
            _headerStyle.fontSize = 8;
            _headerStyle.normal.textColor = Color.gray;
        }
        return _headerStyle;
    }

    private GUIStyle _labelStyle;
    private GUIStyle GetLabelStyle()
    {
        if (_labelStyle == null)
        {
            _labelStyle = new GUIStyle(EditorStyles.miniLabel);
            _labelStyle.alignment = TextAnchor.MiddleCenter;
            _labelStyle.fontSize = 9;
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.fontStyle = FontStyle.Bold;
        }
        return _labelStyle;
    }

    private void SetItemPosition(int x, int z)
    {
        ItemsCollection.ItemData config = _itemsCollection.list[_selectedItemIndex];
        
        // Valid position check
        if (x + config.gridWidth > GridWidth || z + config.gridHeight > GridHeight) return;
        
        // Remove existing mapping for this itemId (since each type has only one position)
        _shopLayoutData.items.RemoveAll(i => i.itemId == config.id);

        ShopLayoutItem newItem = new ShopLayoutItem()
        {
            itemId = config.id,
            posX = x,
            posZ = z
        };

        _shopLayoutData.items.Add(newItem);
        UpdateGrid();
        Repaint();
    }

    private void RemoveItemAt(int x, int z)
    {
        int index = _grid[x, z];
        if (index != -1)
        {
            _shopLayoutData.items.RemoveAt(index);
            UpdateGrid();
            Repaint();
        }
    }
}
