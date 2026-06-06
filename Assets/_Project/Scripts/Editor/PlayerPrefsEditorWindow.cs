using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PlayerPrefsEditorWindow : EditorWindow
{
    private Vector2 scrollPos;
    
    // Core keys from the game
    private readonly string[] intKeys = new string[] {
        "numberOfGoldInStorage",
        "numberOfDiamondsInStorage",
        "numberOfStudentInStorage",
        "numberOfHappyInStorage",
        "numberOfEducationInStorage",
        "currentSemester",
        "currentLevel",
        "totalSpawnedNPCs",
        "hasFinishedFinalTutorial"
    };

    private readonly string[] floatKeys = new string[] {
        "levelProgress",
        "timeRemaining"
    };

    private readonly string[] stringKeys = new string[] {
        "playerTeacherInventory"
    };

    // For custom keys
    private string customKeyName = "";
    private int customKeyTypeIndex = 0; // 0=Int, 1=Float, 2=String
    private string[] customKeyTypes = new string[] { "Int", "Float", "String" };
    private int customIntValue = 0;
    private float customFloatValue = 0f;
    private string customStringValue = "";

    [MenuItem("Tools/PlayerPrefs Manager")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsEditorWindow>("PlayerPrefs Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("Delete All PlayerPrefs", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Delete All", "Are you sure you want to delete ALL PlayerPrefs? This cannot be undone.", "Yes", "No"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("All PlayerPrefs deleted.");
            }
        }

        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawCoreKeys();

        EditorGUILayout.Space();
        DrawCustomKeySection();

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            PlayerPrefs.Save();
        }
    }

    private void DrawCoreKeys()
    {
        GUILayout.Label("Known Keys", EditorStyles.boldLabel);

        // Int Keys
        foreach (string key in intKeys)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, GUILayout.Width(200));
            if (PlayerPrefs.HasKey(key))
            {
                int val = PlayerPrefs.GetInt(key);
                int newVal = EditorGUILayout.IntField(val);
                if (newVal != val) PlayerPrefs.SetInt(key, newVal);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
            else
            {
                GUILayout.Label("Not Set", EditorStyles.miniLabel);
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    PlayerPrefs.SetInt(key, 0);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // Float Keys
        foreach (string key in floatKeys)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, GUILayout.Width(200));
            if (PlayerPrefs.HasKey(key))
            {
                float val = PlayerPrefs.GetFloat(key);
                float newVal = EditorGUILayout.FloatField(val);
                if (newVal != val) PlayerPrefs.SetFloat(key, newVal);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
            else
            {
                GUILayout.Label("Not Set", EditorStyles.miniLabel);
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    PlayerPrefs.SetFloat(key, 0f);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // String Keys
        foreach (string key in stringKeys)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, GUILayout.Width(200));
            if (PlayerPrefs.HasKey(key))
            {
                string val = PlayerPrefs.GetString(key);
                string newVal = EditorGUILayout.TextField(val);
                if (newVal != val) PlayerPrefs.SetString(key, newVal);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
            else
            {
                GUILayout.Label("Not Set", EditorStyles.miniLabel);
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    PlayerPrefs.SetString(key, "");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawCustomKeySection()
    {
        GUILayout.Label("Custom Key (e.g. BuildingTeacher_123)", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        customKeyName = EditorGUILayout.TextField("Key Name", customKeyName);
        customKeyTypeIndex = EditorGUILayout.Popup(customKeyTypeIndex, customKeyTypes, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (customKeyTypeIndex == 0) // Int
        {
            customIntValue = EditorGUILayout.IntField("Value", customIntValue);
            if (GUILayout.Button("Set", GUILayout.Width(60)))
            {
                PlayerPrefs.SetInt(customKeyName, customIntValue);
                PlayerPrefs.Save();
            }
            if (GUILayout.Button("Get", GUILayout.Width(60)))
            {
                customIntValue = PlayerPrefs.GetInt(customKeyName, 0);
            }
        }
        else if (customKeyTypeIndex == 1) // Float
        {
            customFloatValue = EditorGUILayout.FloatField("Value", customFloatValue);
            if (GUILayout.Button("Set", GUILayout.Width(60)))
            {
                PlayerPrefs.SetFloat(customKeyName, customFloatValue);
                PlayerPrefs.Save();
            }
            if (GUILayout.Button("Get", GUILayout.Width(60)))
            {
                customFloatValue = PlayerPrefs.GetFloat(customKeyName, 0f);
            }
        }
        else // String
        {
            customStringValue = EditorGUILayout.TextField("Value", customStringValue);
            if (GUILayout.Button("Set", GUILayout.Width(60)))
            {
                PlayerPrefs.SetString(customKeyName, customStringValue);
                PlayerPrefs.Save();
            }
            if (GUILayout.Button("Get", GUILayout.Width(60)))
            {
                customStringValue = PlayerPrefs.GetString(customKeyName, "");
            }
        }
        
        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            PlayerPrefs.DeleteKey(customKeyName);
            PlayerPrefs.Save();
        }
        EditorGUILayout.EndHorizontal();
    }
}
