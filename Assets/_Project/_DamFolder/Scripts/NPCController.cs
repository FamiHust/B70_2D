using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NPCController : MonoBehaviour
{
    public int id;

    Vector2Int currentCell;
    Queue<Vector2Int> path = new Queue<Vector2Int>();

    float tickTimer = 0f;
    public float tickRate = 0.25f;

    SpriteRenderer sr;
    Transform cachedTransform;

    public int sortingOffset = 4000;

    Transform visual;
    Tween moveTween;

    public float moveDuration = 0.2f;
    public Ease moveEase = Ease.Linear;

    Vector2Int targetCell;

    int stuckTicks = 0;
    const int STUCK_THRESHOLD = 2;

    public Sprite frontSprite;
    public Sprite backSprite;

    void Awake()
    {
        cachedTransform = transform;
        sr = GetComponentInChildren<SpriteRenderer>();
        visual = sr.transform;
    }

    void Start()
    {
        currentCell = WorldToCell(cachedTransform.position);
        NPCGridSystem.Instance.Occupy(currentCell.x, currentCell.y, id);

        UpdateSorting();
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
        targetCell = WorldToCell(target);

        var rawPath = GroundManager.instance.GetPath(cachedTransform.position, target, false);

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
        if (!GroundManager.instance.pathNodesWithoutWall[next.x, next.y])
        {
            RequestNewPath();
            return;
        }

        if (!NPCGridSystem.Instance.IsFree(next.x, next.y))
        {
            HandleBlocked();
            return;
        }

        stuckTicks = 0;

        NPCGridSystem.Instance.Release(currentCell.x, currentCell.y);
        Vector2Int prevCell = currentCell;
        currentCell = next;
        path.Dequeue();

        NPCGridSystem.Instance.Occupy(currentCell.x, currentCell.y, id);

        Vector3 oldWorldPos = cachedTransform.position;
        Vector3 newWorldPos = CellToWorld(currentCell);
        cachedTransform.position = newWorldPos;

        AnimateMove(oldWorldPos, newWorldPos, prevCell, currentCell);
        UpdateSorting();
    }

    void AnimateMove(Vector3 oldWorldPos, Vector3 newWorldPos, Vector2Int from, Vector2Int to)
    {
        if (visual == null || sr == null) return;

        moveTween?.Kill();
        visual.position = oldWorldPos;

        Vector2Int dir = to - from;

        float screenX = dir.x - dir.y;
        float screenY = dir.x + dir.y;

        if (screenY > 0)
        {
            sr.sprite = backSprite;
        }
        else
        {
            sr.sprite = frontSprite;
        }

        if (screenX != 0)
        {
            Vector3 scale = visual.localScale;
            scale.x = screenX < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            visual.localScale = scale;
        }

        moveTween = visual.DOMove(newWorldPos, moveDuration).SetEase(moveEase);
    }

    void UpdateSorting()
    {
        if (sr == null) return;

        int z = currentCell.y;

        sr.sortingOrder = -(z * 100) + sortingOffset;
    }

    void HandleBlocked()
    {
        stuckTicks++;

        if (stuckTicks >= STUCK_THRESHOLD)
        {
            Vector2Int sidestepCell = FindSidestepCell();
            if (sidestepCell != new Vector2Int(-1, -1))
            {
                NPCGridSystem.Instance.Release(currentCell.x, currentCell.y);
                Vector2Int prevCell = currentCell;
                currentCell = sidestepCell;

                path.Clear();

                NPCGridSystem.Instance.Occupy(currentCell.x, currentCell.y, id);

                Vector3 oldWorldPos = cachedTransform.position;
                Vector3 newWorldPos = CellToWorld(currentCell);
                cachedTransform.position = newWorldPos;

                AnimateMove(oldWorldPos, newWorldPos, prevCell, currentCell);
                UpdateSorting();
                stuckTicks = 0;
            }
        }
    }

    Vector2Int FindSidestepCell()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int checkCell = new Vector2Int(currentCell.x + x, currentCell.y + y);

                if (checkCell.x >= 0 && checkCell.x < GroundManager.nodeWidth &&
                    checkCell.y >= 0 && checkCell.y < GroundManager.nodeHeight &&
                    GroundManager.instance.pathNodesWithoutWall[checkCell.x, checkCell.y] &&
                    NPCGridSystem.Instance.IsFree(checkCell.x, checkCell.y))
                {
                    return checkCell;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    Vector2Int WorldToCell(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x, 0, cell.y);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, CellToWorld(targetCell) + Vector3.up * 0.5f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(CellToWorld(targetCell) + Vector3.up * 0.5f, new Vector3(0.8f, 0.8f, 0.8f));

            if (path != null && path.Count > 0)
            {
                Gizmos.color = Color.red;
                Vector2Int[] nodes = path.ToArray();
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    Gizmos.DrawLine(CellToWorld(nodes[i]) + Vector3.up * 0.2f, CellToWorld(nodes[i + 1]) + Vector3.up * 0.2f);
                }
            }
        }
    }
}