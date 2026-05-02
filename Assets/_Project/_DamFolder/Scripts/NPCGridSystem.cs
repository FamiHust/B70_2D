using UnityEngine;

public class NPCGridSystem : MonoBehaviour
{
    public static NPCGridSystem Instance;

    int[,] occupancy;

    void Awake()
    {
        Instance = this;
        occupancy = new int[GroundManager.nodeWidth, GroundManager.nodeHeight];
        Clear();
    }

    public void Clear()
    {
        for (int x = 0; x < GroundManager.nodeWidth; x++)
            for (int z = 0; z < GroundManager.nodeHeight; z++)
                occupancy[x, z] = -1;
    }

    public bool IsFree(int x, int z)
    {
        return occupancy[x, z] == -1;
    }

    public int GetOccupant(int x, int z)
    {
        return occupancy[x, z];
    }

    public void Occupy(int x, int z, int id)
    {
        occupancy[x, z] = id;
    }

    public void Release(int x, int z)
    {
        occupancy[x, z] = -1;
    }
}