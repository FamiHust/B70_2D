using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBubbleManager : MonoBehaviour
{
    public static NPCBubbleManager instance;

    [Header("Bubble Prefab")]
    [Tooltip("Prefab with NPCSpeechBubbleScript. Will be instantiated under each NPC.")]
    public GameObject SpeechBubblePrefab;

    [Header("Complaint Bubble Settings")]
    [Tooltip("Min seconds between complaint bubble appearances.")]
    public float complaintIntervalMin = 30f;
    [Tooltip("Max seconds between complaint bubble appearances.")]
    public float complaintIntervalMax = 60f;
    [Tooltip("How long the complaint bubble stays visible.")]
    public float complaintDuration = 10f;

    [Header("Bubble Scale & Offset")]
    [Tooltip("If the bubble is too small, increase this multiplier (e.g. 2, 3, or 5).")]
    public float bubbleScaleMultiplier = 2.5f;
    [Tooltip("How high above the NPC's pivot the bubble should appear.")]
    public float bubbleOffsetY = 3.0f;

    [Header("Event Bubble Settings (3 dots)")]
    [Tooltip("Min seconds between '...' bubble appearances.")]
    public float eventIntervalMin = 60f;
    [Tooltip("Max seconds between '...' bubble appearances.")]
    public float eventIntervalMax = 120f;

    private NPCSpeechBubbleScript _activeComplaintBubble;
    private NPCSpeechBubbleScript _activeEventBubble;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(_ComplaintLoop());
        StartCoroutine(_EventLoop());
    }

    private IEnumerator _ComplaintLoop()
    {
        
        yield return new WaitForSeconds(2f);

        while (true)
        {
            float waitTime = Random.Range(complaintIntervalMin, complaintIntervalMax);
            yield return new WaitForSeconds(waitTime);

            if (SceneManager.instance == null || SceneManager.instance.gameMode != Common.GameMode.NORMAL)
            {
                Debug.Log("[NPCBubbleManager] Complaint Skipped! gameMode = " + (SceneManager.instance != null ? SceneManager.instance.gameMode.ToString() : "null"));
                continue;
            }

            List<NPCController> npcs = _GetWalkingNPCs();
            if (npcs.Count == 0) continue;

            int spawnCount = 1;
            for (int i = 0; i < spawnCount && npcs.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, npcs.Count);
                NPCController npc = npcs[randomIndex];
                npcs.RemoveAt(randomIndex); 

                string randomComplaint = NPCEventData.Complaints[Random.Range(0, NPCEventData.Complaints.Count)];
                _SpawnBubble(npc, randomComplaint);
            }
        }
    }

    private IEnumerator _EventLoop()
    {
        yield return new WaitForSeconds(5f); 

        while (true)
        {
            float waitTime = Random.Range(eventIntervalMin, eventIntervalMax);
            yield return new WaitForSeconds(waitTime);

            if (SceneManager.instance == null || SceneManager.instance.gameMode != Common.GameMode.NORMAL)
            {
                continue;
            }

            List<NPCController> npcs = _GetWalkingNPCs();
            if (npcs.Count == 0) continue;

            int randomIndex = Random.Range(0, npcs.Count);
            NPCController npc = npcs[randomIndex];

            _SpawnEventBubble(npc);
        }
    }

    private List<NPCController> _GetWalkingNPCs()
    {
        List<NPCController> result = new List<NPCController>();
        NPCController[] allNPCs = FindObjectsOfType<NPCController>();
        
        foreach (var npc in allNPCs)
        {
            if (npc != null && npc.gameObject.activeInHierarchy)
            {

                var bubble = npc.GetComponentInChildren<NPCSpeechBubbleScript>(false);
                if (bubble == null || !bubble.gameObject.activeInHierarchy)
                {
                    result.Add(npc);
                }
            }
        }
        return result;
    }

    private void _SpawnBubble(NPCController npc, string message)
    {
        if (SpeechBubblePrefab == null) return;
        if (npc == null || npc.gameObject == null) return;

        _ShowBubble(npc, ref _activeComplaintBubble, (bubble) =>
        {
            bubble.ShowComplaint(message, complaintDuration);
        });
    }

    private void _SpawnEventBubble(NPCController npc)
    {
        if (SpeechBubblePrefab == null) return;
        if (npc == null || npc.gameObject == null) return;

        _ShowBubble(npc, ref _activeEventBubble, (bubble) =>
        {
            bubble.ShowEllipsis();
            
            bubble.OnEllipsisTapped = null;
            bubble.OnEllipsisTapped += () =>
            {
                
                bubble.Hide();
                if (bubble != null && bubble.gameObject != null)
                    Destroy(bubble.gameObject);
            };
        });
    }

    private void _ShowBubble(NPCController npc, ref NPCSpeechBubbleScript bubbleRef, System.Action<NPCSpeechBubbleScript> setup)
    {
        if (SpeechBubblePrefab == null)
        {
            Debug.LogWarning("[NPCBubbleManager] SpeechBubblePrefab is not assigned!");
            return;
        }

        if (bubbleRef != null && bubbleRef.gameObject != null)
        {
            bubbleRef.Hide();
            Destroy(bubbleRef.gameObject);
        }

        SpriteRenderer sr = npc.GetComponentInChildren<SpriteRenderer>();
        GameObject uiRoot = sr != null ? sr.gameObject : npc.gameObject;

        GameObject inst = Instantiate(SpeechBubblePrefab, uiRoot.transform);

        inst.transform.localPosition = new Vector3(0f, bubbleOffsetY, 0f);

        Vector3 parentScale = uiRoot.transform.lossyScale;
        Vector3 prefScale = SpeechBubblePrefab.transform.localScale;
        inst.transform.localScale = new Vector3(
            (prefScale.x / parentScale.x) * bubbleScaleMultiplier,
            (prefScale.y / parentScale.y) * bubbleScaleMultiplier,
            (prefScale.z / parentScale.z) * bubbleScaleMultiplier
        );
        bubbleRef = inst.GetComponent<NPCSpeechBubbleScript>();

        if (bubbleRef == null)
        {
            Debug.LogError("[NPCBubbleManager] SpeechBubblePrefab is missing NPCSpeechBubbleScript!");
            return;
        }

        setup(bubbleRef);
    }

    public void DismissAll()
    {
        
        NPCSpeechBubbleScript[] allBubbles = FindObjectsOfType<NPCSpeechBubbleScript>();
        foreach (var b in allBubbles)
        {
            if (b != null)
            {
                b.Hide();
                Destroy(b.gameObject);
            }
        }
        
        _activeComplaintBubble = null;
        _activeEventBubble = null;
    }
}
