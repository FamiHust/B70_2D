using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    public List<GameObject> npcPrefabs;
    public Transform npcContainer;

    [Header("Status")]
    [SerializeField] private int nextId = 0;

    public void SpawnRandomNPC()
    {
        Vector3 spawnPos = GroundManager.instance.GetRandomFreePosition();

        int randomIndex = Random.Range(0, npcPrefabs.Count);
        GameObject prefabToSpawn = npcPrefabs[randomIndex];

        GameObject newNPC = Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);

        if (npcContainer != null) newNPC.transform.SetParent(npcContainer);

        NPCController controller = newNPC.GetComponent<NPCController>();
        if (controller != null)
        {
            controller.id = nextId++;
        }

    }
}