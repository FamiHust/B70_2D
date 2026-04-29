using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GroundManager))]
public class GroundManagerEditor : Editor
{
    private bool editMode = false;
    private GroundManager manager;

    void OnEnable()
    {
        manager = (GroundManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Road Editor Tools", EditorStyles.boldLabel);

        editMode = GUILayout.Toggle(editMode, "Enable Road Paint Mode", "Button");

        if (GUILayout.Button("Clear All Roads"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Xóa toàn bộ đường đã vẽ?", "Yes", "No"))
            {
                manager.serializedRoadData = new bool[GroundManager.nodeWidth * GroundManager.nodeHeight];
                manager.UpdateAllNodes();
                EditorUtility.SetDirty(manager);
            }
        }

        if (GUI.changed) EditorUtility.SetDirty(manager);
    }

    void OnSceneGUI()
    {
        if (!editMode) return;

        // Vô hiệu hóa việc chọn object khác khi đang vẽ
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);

            Handles.color = Color.yellow;
            Handles.DrawWireCube(new Vector3(x + 0.5f, 0, z + 0.5f), new Vector3(1, 0.1f, 1));

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
            {
                manager.SetRoad(x, z, !e.shift);
                e.Use();
                EditorUtility.SetDirty(manager);
            }
        }

        SceneView.RepaintAll();
    }
}