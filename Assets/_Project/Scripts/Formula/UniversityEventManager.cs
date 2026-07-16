using System.Collections.Generic;
using UnityEngine;

namespace B70.Balance
{
    /// <summary>
    /// Quản lý việc roll ngẫu nhiên và áp dụng các Event trong game.
    /// Có thể đính kèm vào bất kỳ GameObject nào (như GameManager hoặc SceneManager object).
    /// </summary>
    public class UniversityEventManager : MonoBehaviour
    {
        public static UniversityEventManager instance;

        [Header("Event Database")]
        [Tooltip("Danh sách các sự kiện ngẫu nhiên có thể xảy ra trong kỳ")]
        public List<UniversityEventData> availableEvents = new List<UniversityEventData>();

        [Header("Prefabs")]
        public GameObject eventPrefab;
        [Tooltip("Container (UI Panel hoặc Group) để chứa các prefab event được sinh ra")]
        public Transform eventContainer;

        [Header("Spawn Settings")]
        [Tooltip("Xác suất xuất hiện sự kiện mỗi khi kiểm tra (0.0 đến 1.0)")]
        [Range(0f, 1f)]
        public float eventProbabilityPerSemester = 0.5f;

        [Tooltip("Số sự kiện tối đa trong 1 kỳ")]
        public int maxEventsPerSemester = 2;

        [Tooltip("Khoảng chờ (giây) giữa 2 sự kiện liên tiếp")]
        public float cooldownBetweenEvents = 10f;

        private int currentSemesterEventsSpawned = 0;
        private int lastSemester = -1;
        private float nextEventTime = -1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Logic này đã được thay thế bằng hệ thống per-building trong ProductionScript.
            // Mỗi tòa nhà tự theo dõi và roll event thông qua UpdateEventTiming().
            // Để giữ backward compat, có thể dùng lại nếu cần global event (không gắn vào tòa nào).
        }

        private void CalculateNextEvent()
        {
            if (currentSemesterEventsSpawned >= maxEventsPerSemester)
            {
                nextEventTime = -1f;
                return;
            }

            // Tung xúc xắc xem event tiếp theo có xuất hiện hay không
            if (Random.Range(0f, 1f) <= eventProbabilityPerSemester)
            {
                float timeRemaining = TimeManager.instance.timeRemaining;
                // maxTriggerTime là thời điểm sớm nhất có thể trigger (vì đếm ngược, timeRemaining càng nhỏ là càng về cuối kỳ)
                // Cần đảm bảo event xuất hiện trước khi kỳ kết thúc, và cách event trước ít nhất cooldownBetweenEvents
                float maxTriggerTime = timeRemaining - cooldownBetweenEvents;

                if (maxTriggerTime > 0)
                {
                    // Random thời điểm xuất hiện (từ lúc hết cooldown đến khi kết thúc kỳ)
                    nextEventTime = Random.Range(0f, maxTriggerTime);
                }
                else
                {
                    nextEventTime = -1f; // Không đủ thời gian cooldown, bỏ qua
                }
            }
            else
            {
                nextEventTime = -1f;
            }
        }

        /// <summary>
        /// Gọi thủ công một event cụ thể trong danh sách availableEvents.
        /// </summary>
        public void TriggerSpecificEvent(int index)
        {
            if (index < 0 || index >= availableEvents.Count) return;
            
            var evt = availableEvents[index];
            SpawnEventPrefab(evt);
                
            Debug.Log($"[Specific Event Triggered] {evt.eventName}");
        }

        /// <summary>
        /// Hàm bổ trợ để áp dụng các chỉ số của một Option trong event vào SceneManager.
        /// </summary>
        public void ApplyOptionEffects(EventOption option)
        {
            if (SceneManager.instance != null)
            {
                SceneManager.instance.numberOfGoldInStorage += option.goldModifier;
                
                SceneManager.instance.numberOfHappyInStorage = Mathf.Clamp(
                    SceneManager.instance.numberOfHappyInStorage + Mathf.RoundToInt(option.happinessModifier), 
                    0, 
                    SceneManager.instance.happyStorageCapacity);
                    
                SceneManager.instance.numberOfEducationInStorage = Mathf.Clamp(
                    SceneManager.instance.numberOfEducationInStorage + Mathf.RoundToInt(option.educationModifier), 
                    0, 
                    SceneManager.instance.educationStorageCapacity);
            }
        }

        /// <summary>
        /// [Mới] Mở Event Window gắn với một tòa nhà cụ thể.
        /// Được gọi bởi ProductionScript.TriggerEvent().
        /// </summary>
        public void ShowEventForBuilding(UniversityEventData evt, BaseItemScript sourceBuilding)
        {
            if (evt == null) return;
            if (CameraManager.instance != null && CameraManager.instance.isZoomLocked) return;

            if (eventPrefab == null)
            {
                Debug.LogWarning("[UniversityEventManager] eventPrefab is not assigned!");
                return;
            }

            if (UIManager.instance != null)
            {
                WindowScript window = UIManager.instance.ShowWindow(eventPrefab);
                UniversityEvent eventComponent = window.GetComponent<UniversityEvent>();
                if (eventComponent != null)
                {
                    eventComponent.Setup(evt, sourceBuilding);
                }
            }
            else
            {
                GameObject inst = Instantiate(eventPrefab, eventContainer != null ? eventContainer : this.transform);
                UniversityEvent eventComponent = inst.GetComponent<UniversityEvent>();
                if (eventComponent != null)
                {
                    eventComponent.Setup(evt, sourceBuilding);
                }
            }
        }

        /// <summary>
        /// Khởi tạo prefab cho event (legacy — dùng cho global events).
        /// </summary>
        private void SpawnEventPrefab(UniversityEventData evt)
        {
            if (CameraManager.instance != null && CameraManager.instance.isZoomLocked)
            {
                return; // Bỏ qua hoàn toàn event này khi đang zoom out
            }

            if (eventPrefab != null)
            {
                if (UIManager.instance != null)
                {
                    // Show as a managed window via UIManager
                    WindowScript window = UIManager.instance.ShowWindow(eventPrefab);
                    UniversityEvent eventComponent = window.GetComponent<UniversityEvent>();
                    if (eventComponent != null)
                    {
                        eventComponent.Setup(evt);
                    }
                }
                else
                {
                    // Fallback to basic instantiation if UIManager is missing
                    GameObject inst = Instantiate(eventPrefab, eventContainer != null ? eventContainer : this.transform);
                    UniversityEvent eventComponent = inst.GetComponent<UniversityEvent>();
                    if (eventComponent != null)
                    {
                        eventComponent.Setup(evt);
                    }
                }
            }
            else
            {
                Debug.LogWarning("[UniversityEventManager] eventPrefab is not assigned! Cannot show event window.");
            }
        }

        /// <summary>
        /// Gọi hàm này tại thời điểm cuối kỳ (vd: trong SceneManager.CompleteSemester).
        /// Trả về danh sách các event đã thực sự xảy ra để hiển thị lên Popup UI.
        /// </summary>
        public List<UniversityEventData> RollAndApplyRandomEvents()
        {
            List<UniversityEventData> triggeredEvents = new List<UniversityEventData>();

            if (SceneManager.instance == null) return triggeredEvents;

            if (SceneManager.instance.currentLevel < 3) return triggeredEvents;

            Debug.Log($"[UniversityEventManager] Rolling random events. Total available: {availableEvents.Count}");

            // Xóa các event cũ trong container trước khi roll mới (nếu cần)
            if (eventContainer != null)
            {
                foreach (Transform child in eventContainer)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var evt in availableEvents)
            {
                if (evt == null) continue;

                // Tung xúc xắc ngẫu nhiên từ 0.0 đến 1.0
                float roll = Random.Range(0f, 1f);

                if (roll <= evt.triggerProbability)
                {
                    // Gọi prefab ra màn hình
                    SpawnEventPrefab(evt);

                    triggeredEvents.Add(evt);
                    
                    Debug.Log($"[Event Triggered] {evt.eventName} (Has Options: {evt.options.Count})");
                }
                else
                {
                    // Debug.Log($"[UniversityEventManager] {evt.eventName} did not trigger (roll: {roll:F2} > prob: {evt.triggerProbability})");
                }
            }

            // Trả về danh sách các event (không lưu resources ở đây vì chờ người dùng accept từng cái)

            return triggeredEvents;
        }
        /// <summary>
        /// Hàm dành cho Button UI: Kích hoạt ngay lập tức 1 event ngẫu nhiên từ danh sách (bỏ qua xác suất).
        /// </summary>
        public void OnClickTriggerRandomEvent()
        {
            if (availableEvents == null || availableEvents.Count == 0)
            {
                Debug.LogWarning("[UniversityEventManager] No events available to trigger!");
                return;
            }

            // Chọn ngẫu nhiên 1 event
            var evt = availableEvents[Random.Range(0, availableEvents.Count)];
            if (evt == null) return;


            Debug.Log($"[UniversityEventManager] Manually triggered event prefab: {evt.eventName}");
            SpawnEventPrefab(evt);
        }

        /// <summary>
        /// Thử kích hoạt duy nhất một event ngẫu nhiên từ danh sách (vẫn xét xác suất).
        /// </summary>
        public void RollOneRandomEvent()
        {
            if (availableEvents == null || availableEvents.Count == 0) return;

            var evt = availableEvents[Random.Range(0, availableEvents.Count)];
            if (evt == null) return;

            float roll = Random.Range(0f, 1f);
            if (roll <= evt.triggerProbability)
            {
                SpawnEventPrefab(evt);
            }
        }

        /// <summary>
        /// Kích hoạt 1 event ngẫu nhiên (chắc chắn xuất hiện) cho logic Update trong kỳ.
        /// </summary>
        public void ForceSpawnOneRandomEvent()
        {
            if (availableEvents == null || availableEvents.Count == 0) return;

            var evt = availableEvents[Random.Range(0, availableEvents.Count)];
            if (evt != null)
            {
                SpawnEventPrefab(evt);
            }
        }

        [Header("Event Focus Mode")]
        public bool isEventFocused = false;
        public BaseItemScript eventFocusBuilding = null;

        public void StartEventFocus(BaseItemScript building)
        {
            if (isEventFocused) return;
            if (PlayerPrefs.GetInt("hasResolvedFirstEvent", 0) == 1) return; // Chỉ thực hiện với event đầu tiên của game

            isEventFocused = true;
            eventFocusBuilding = building;

            if (UIManager.instance != null)
            {
                UIManager.instance.CloseAllWindowsExceptOverlay();
                if (GameOverlayWindowScript.instance != null)
                {
                    GameOverlayWindowScript.instance.HideOverlay();
                }
            }

            if (TimeManager.instance != null)
            {
                TimeManager.instance.SetPaused(true);
            }

            if (CameraManager.instance != null)
            {
                CameraManager.instance.FocusOnItem(building, 8f);
            }
        }

        public void StopEventFocus()
        {
            isEventFocused = false;
            eventFocusBuilding = null;

            PlayerPrefs.SetInt("hasResolvedFirstEvent", 1);
            PlayerPrefs.Save();

            if (UIManager.instance != null)
            {
                if (GameOverlayWindowScript.instance != null)
                {
                    GameOverlayWindowScript.instance.ShowOverlay();
                }
            }

            if (TimeManager.instance != null)
            {
                TimeManager.instance.SetPaused(false);
            }
        }
    }
}
