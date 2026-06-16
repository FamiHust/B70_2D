using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner instance;

    [Header("Spawn Settings")]
    public int npcSpawnedAtLevel2 = 5;
    public int npcSpawnedPerSemester = 2;

    [Header("Pool Settings")]
    public List<GameObject> npcPrefabs;
    public Transform npcContainer;
    public int initialPoolSize = 20;

    [Header("Area Config")]
    public Vector2 areaCenter = Vector2.zero;
    public Vector2 areaSize = new Vector2(40, 40);

    private List<NPCController> npcPool = new List<NPCController>();

    [Header("Status")]
    [SerializeField] private int nextId = 0;

    void Awake()
    {
        instance = this;
    }

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

    public void SpawnNPCs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomNPC();
        }
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

        Vector3 spawnPos = GetRandomNPCPosition();
        
        npcToSpawn.Spawn(spawnPos);
    }

    public bool IsCellInNPCSpawnArea(int x, int z)
    {
        if (GroundManager.instance == null) return false;

        int minX = Mathf.RoundToInt(areaCenter.x - areaSize.x / 2f) - GroundManager.instance.gridOriginX;
        int maxX = Mathf.RoundToInt(areaCenter.x + areaSize.x / 2f) - GroundManager.instance.gridOriginX;
        int minZ = Mathf.RoundToInt(areaCenter.y - areaSize.y / 2f) - GroundManager.instance.gridOriginZ;
        int maxZ = Mathf.RoundToInt(areaCenter.y + areaSize.y / 2f) - GroundManager.instance.gridOriginZ;

        minX = Mathf.Clamp(minX, 0, GroundManager.nodeWidth - 1);
        maxX = Mathf.Clamp(maxX, 0, GroundManager.nodeWidth - 1);
        minZ = Mathf.Clamp(minZ, 0, GroundManager.nodeHeight - 1);
        maxZ = Mathf.Clamp(maxZ, 0, GroundManager.nodeHeight - 1);

        return x >= minX && x <= maxX && z >= minZ && z <= maxZ;
    }

    public Vector3 GetRandomNPCPosition()
    {
        if (GroundManager.instance == null || NPCGridSystem.Instance == null) return Vector3.zero;

        int minX = Mathf.RoundToInt(areaCenter.x - areaSize.x / 2f) - GroundManager.instance.gridOriginX;
        int maxX = Mathf.RoundToInt(areaCenter.x + areaSize.x / 2f) - GroundManager.instance.gridOriginX;
        int minZ = Mathf.RoundToInt(areaCenter.y - areaSize.y / 2f) - GroundManager.instance.gridOriginZ;
        int maxZ = Mathf.RoundToInt(areaCenter.y + areaSize.y / 2f) - GroundManager.instance.gridOriginZ;

        minX = Mathf.Clamp(minX, 0, GroundManager.nodeWidth - 1);
        maxX = Mathf.Clamp(maxX, 0, GroundManager.nodeWidth - 1);
        minZ = Mathf.Clamp(minZ, 0, GroundManager.nodeHeight - 1);
        maxZ = Mathf.Clamp(maxZ, 0, GroundManager.nodeHeight - 1);

        // First attempt: try to find a completely free node (no buildings, walkable by NPC, and no other NPC)
        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(minX, maxX);
            int z = Random.Range(minZ, maxZ);

            if (GroundManager.instance.instanceNodes[x, z] == -1 && 
                GroundManager.instance.pathNodesNPC[x, z] && 
                NPCGridSystem.Instance.IsFree(x, z))
            {
                return new Vector3(x + GroundManager.instance.gridOriginX, 0, z + GroundManager.instance.gridOriginZ);
            }
        }
        
        // Second attempt: try to find a walkable node (no buildings, walkable by NPC) even if occupied by another NPC
        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(minX, maxX);
            int z = Random.Range(minZ, maxZ);

            if (GroundManager.instance.instanceNodes[x, z] == -1 && 
                GroundManager.instance.pathNodesNPC[x, z])
            {
                return new Vector3(x + GroundManager.instance.gridOriginX, 0, z + GroundManager.instance.gridOriginZ);
            }
        }

        // Third attempt: linear search for any walkable node in the area
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                if (GroundManager.instance.instanceNodes[x, z] == -1 && 
                    GroundManager.instance.pathNodesNPC[x, z])
                {
                    return new Vector3(x + GroundManager.instance.gridOriginX, 0, z + GroundManager.instance.gridOriginZ);
                }
            }
        }
        
        return GroundManager.instance.GetRandomFreePosition(); // fallback
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Vector3 center = new Vector3(areaCenter.x, 0.1f, areaCenter.y);
        Vector3 size = new Vector3(areaSize.x, 0.1f, areaSize.y);
        Gizmos.DrawCube(center, size);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
}