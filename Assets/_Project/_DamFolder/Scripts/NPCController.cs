using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int id;

    Vector2Int currentCell;
    Queue<Vector2Int> path = new Queue<Vector2Int>();

    float tickTimer = 0f;
    public float tickRate = 0.2f;

    void Start()
    {
        currentCell = WorldToCell(transform.position);
        NPCGridSystem.Instance.Occupy(currentCell.x, currentCell.y, id);

        RequestNewPath();
    }

    void Update()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;
            TickMove();
        }
    }

    void RequestNewPath()
    {
        Vector3 target = GroundManager.instance.GetRandomFreePosition();

        var rawPath = GroundManager.instance.GetPath(transform.position, target, false);

        path.Clear();

        foreach (var p in rawPath.nodes)
        {
            path.Enqueue(WorldToCell(p));
        }
    }

    void TickMove()
    {
        if (path.Count == 0)
        {
            RequestNewPath();
            return;
        }

        Vector2Int next = path.Peek();

        // check walkable (tránh building)
        if (!GroundManager.instance.pathNodesWithoutWall[next.x, next.y])
        {
            RequestNewPath();
            return;
        }

        if (!NPCGridSystem.Instance.IsFree(next.x, next.y))
        {
            // cell bị chiếm
            HandleBlocked();
            return;
        }

        // move
        NPCGridSystem.Instance.Release(currentCell.x, currentCell.y);

        currentCell = next;
        path.Dequeue();

        NPCGridSystem.Instance.Occupy(currentCell.x, currentCell.y, id);

        transform.position = CellToWorld(currentCell);
    }

    void HandleBlocked()
    {
        // Cách đơn giản: chờ
        // (stable nhất)

        // nâng cấp sau:
        // - random side step
        // - priority
    }

    Vector2Int WorldToCell(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x, 0, cell.y);
    }
}