using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCSpawner))]
public class NPCSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NPCSpawner spawner = (NPCSpawner)target;

        GUILayout.Space(10);
        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Spawn Random NPC", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                spawner.SpawnRandomNPC();
            }
            else
            {
                Debug.LogWarning("Press Play first");
            }
        }
        GUI.backgroundColor = Color.white;
    }
}