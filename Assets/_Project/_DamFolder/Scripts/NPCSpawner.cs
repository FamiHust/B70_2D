using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    public List<GameObject> npcPrefabs;
    public Transform npcContainer;
    public int initialPoolSize = 20;

    private List<NPCController> npcPool = new List<NPCController>();

    [Header("Status")]
    [SerializeField] private int nextId = 0;



    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewNPC();
        }
    }

    private NPCController CreateNewNPC()
    {
        int randomIndex = Random.Range(0, npcPrefabs.Count);
        GameObject prefabToSpawn = npcPrefabs[randomIndex];
        GameObject newNPC = Instantiate(prefabToSpawn, Vector3.zero, prefabToSpawn.transform.rotation);

        if (npcContainer != null) newNPC.transform.SetParent(npcContainer);

        NPCController controller = newNPC.GetComponent<NPCController>();
        if (controller != null)
        {
            controller.id = nextId++;
            npcPool.Add(controller);
        }

        newNPC.SetActive(false);
        return controller;
    }

    public void SpawnRandomNPC()
    {
        NPCController npcToSpawn = null;
        
        foreach (var npc in npcPool)
        {
            if (!npc.gameObject.activeInHierarchy)
            {
                npcToSpawn = npc;
                break;
            }
        }

        if (npcToSpawn == null)
        {
            npcToSpawn = CreateNewNPC();
        }

        Vector3 spawnPos = GroundManager.instance.GetRandomFreePosition();
        
        npcToSpawn.Spawn(spawnPos);
    }
}