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

    void Awake()
    {
        cachedTransform = transform;

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("NPCController: Missing SpriteRenderer!");
            return;
        }

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
        if (visual == null) return;

        moveTween?.Kill();

        visual.position = oldWorldPos;

        Vector2Int dir = to - from;

        if (dir.x != 0)
        {
            Vector3 scale = visual.localScale;
            scale.x = dir.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            visual.localScale = scale;
        }

        moveTween = visual.DOMove(newWorldPos, moveDuration)
            .SetEase(moveEase);
    }

    void UpdateSorting()
    {
        if (sr == null) return;

        int z = currentCell.y;

        sr.sortingOrder = -(z * 100) + sortingOffset;
    }

    void HandleBlocked()
    {

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