using System;
using UnityEngine;
using B70.Balance;

public class ProductionScript : MonoBehaviour
{
    /* public vars */
    public bool readyForCollection = false;
    public int collectedAmount = 0;
    public bool isUnderConstruction = false;

    /* event vars */
    public bool readyForEvent = false;
    private B70.Balance.UniversityEventData _pendingEvent = null;
    private int _lastEventSemester = -1;
    private float _nextEventTime = -1f;
    private string _currentResourceType = ""; // icon resource đang hiển khi không có event

    /* private vars */
    private BaseItemScript _baseItem;
    private float _productionRate = 0.0f;
    private string _productType;
    private int _productPrice;
    
    [Header("Construction")]
    public float constructionTimeTotal;
    public float constructionTimeRemaining;

    // ⚠️ Dùng Unix Time để lưu lâu dài (không bị reset khi restart game)
    private double _lastCollectedTime = 0;

    public void SetData(BaseItemScript baseItem, double lastCollectedTime = 0)
    {
        this._baseItem = baseItem;
        this._productionRate = baseItem.itemData.configuration.productionRate;
        this._productType = baseItem.itemData.configuration.product;
        this._productPrice = baseItem.itemData.configuration.productPrice;

        this.constructionTimeTotal = baseItem.itemData.configuration.buildTime;
        this.constructionTimeRemaining = this.constructionTimeTotal;

        this._lastCollectedTime = lastCollectedTime;
        // Nếu chưa có dữ liệu (spawn mới) hoặc không có dữ liệu cũ được truyền vào
        if (_lastCollectedTime == 0)
            _lastCollectedTime = GetCurrentTime();
    }

    public void OnConstructionFinished()
    {
        this.isUnderConstruction = false;
        this._lastCollectedTime = GetCurrentTime();
        if (this._baseItem != null)
            DataBaseManager.instance.UpdateItemData(this._baseItem);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SoundData.SFX_Complete_Build);
        }

        // Tutorial handling: when the first building is finished
        if (SceneManager.instance != null && SceneManager.instance.isTutorialActive)
        {
            if (SceneManager.instance.GetBuildingCount() == 1)
            {
                // Show TutorialWindow again
                TutorialWindowScript tut = TutorialWindowScript.instance;
                if (tut == null && UIManager.instance != null)
                {
                    tut = UIManager.instance.ShowTutorialWindow() as TutorialWindowScript;
                }

                if (tut != null)
                {
                    tut.ShowWindow();
                    tut.SetTutorialContent("Tòa nhà đã xây xong! Hãy nhấn vào Mission để nhận thưởng.");
                    tut.ShowCharacter(false);
                    tut.transform.SetAsLastSibling();
                }

                // Show GameOverlay in Mission tutorial state
                if (GameOverlayWindowScript.instance != null)
                {
                    GameOverlayWindowScript.instance.ShowOverlay();
                    GameOverlayWindowScript.instance.SetMissionTutorialState(true);
                    GameOverlayWindowScript.instance.transform.SetAsLastSibling();
                }
            }
        }

        if (SceneManager.instance != null)
        {
            SceneManager.instance.UpdateStudentStorageCapacity();  // tăng cap AFTER xây xong
            SceneManager.instance.UpdateLevelProgress();
        }
    }

    private void Update()
    {
        if (this.isUnderConstruction)
        {
            if (this.constructionTimeRemaining > 0)
            {
                this.constructionTimeRemaining -= Time.deltaTime;
                if (this.constructionTimeRemaining <= 0)
                {
                    this.constructionTimeRemaining = 0;
                    this.OnConstructionFinished();
                }
            }
        }
        else
        {
            this.UpdateProduction();
            this.UpdateEventTiming();
        }
    }

    public float GetEffectiveProductionRate(string productType)
    {
        float rate = this._productionRate;
        if (this._baseItem != null && this._baseItem.assignedTeacher != null)
        {
            if (productType == "gold")
            {
                rate *= this._baseItem.assignedTeacher.influenceGold;
            }
            else if (productType == "happy")
            {
                rate *= (1f + this._baseItem.assignedTeacher.influenceHappy);
            }
            else if (productType == "education" || productType == "academic" || productType == "edu")
            {
                rate *= (1f + this._baseItem.assignedTeacher.influenceEducation);
            }
        }
        return rate;
    }

    public int GetEffectiveProductPrice(string productType)
    {
        float price = this._productPrice;
        if (this._baseItem != null && this._baseItem.assignedTeacher != null)
        {
            if (productType == "gold")
            {
                price += this._baseItem.assignedTeacher.influenceGold;
            }
            else if (productType == "happy")
            {
                price *= (1f + this._baseItem.assignedTeacher.influenceHappy);
            }
            else if (productType == "education" || productType == "academic" || productType == "edu")
            {
                price *= (1f + this._baseItem.assignedTeacher.influenceEducation);
            }
        }
        return Mathf.RoundToInt(price);
    }

    public void UpdateProduction()
    {
        if (this.isUnderConstruction || (TimeManager.instance != null && TimeManager.instance.isPaused))
            return;

        double time = GetCurrentTime() - this._lastCollectedTime;
        bool anyReady = false;
        string firstReadyType = "";

        if (!string.IsNullOrEmpty(this._productType))
        {
            string[] products = this._productType.Split(',');
            foreach (string p in products)
            {
                string pType = p.Trim();
                if (string.IsNullOrEmpty(pType)) continue;

                int productAmount = (int)((time / 3600.0) * GetEffectiveProductionRate(pType));
                if (productAmount >= 1)
                {
                    anyReady = true;
                    if (string.IsNullOrEmpty(firstReadyType)) firstReadyType = pType;
                }
            }
        }

        if (anyReady && !readyForCollection)
        {
            readyForCollection = true;
            _currentResourceType = firstReadyType;
            // Chỉ hiển icon resource nếu không có event đang chờ
            if (!readyForEvent)
            {
                this._baseItem.UI.ShowCollectNotificationUI(true, firstReadyType);
            }
        }
    }

    public void Collect()
    {
        double time = GetCurrentTime() - this._lastCollectedTime;
        if (string.IsNullOrEmpty(this._productType)) return;

        string[] products = this._productType.Split(',');
        bool collectedAnything = false;
        string firstProduct = "";

        foreach (string p in products)
        {
            string pType = p.Trim();
            if (string.IsNullOrEmpty(pType)) continue;

            int productAmount = (int)((time / 3600.0) * GetEffectiveProductionRate(pType));
            if (productAmount > 0)
            {
                SceneManager.instance.CollectResource(pType, productAmount);
                collectedAnything = true;
                if (string.IsNullOrEmpty(firstProduct)) firstProduct = pType;
            }
        }

        if (collectedAnything)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(SoundData.SFX_Collect_Product);
            }
            this._baseItem.Particles.ShowCollectionParticle(firstProduct);
            this._baseItem.UI.ShowCollectNotificationUI(false, firstProduct);

            this._lastCollectedTime = GetCurrentTime();
            this.readyForCollection = false;
            _currentResourceType = "";

            DataBaseManager.instance.UpdateItemData(this._baseItem);
        }
    }

    // ========================
    // EVENT LOGIC
    // ========================

    /// <summary>
    /// Tính toán xem kỳ này tòa nhà có xảy ra event không và lúc nào.
    /// Gọi một lần mỗi kỳ khi kỳ mới bắt đầu.
    /// </summary>
    private void CalculateNextEventForBuilding()
    {
        _nextEventTime = -1f;
        _pendingEvent = null;

        if (_baseItem == null || _baseItem.itemData == null)
        {
            Debug.Log($"[EventTiming] {name}: _baseItem hoặc itemData là null");
            return;
        }
        if (_baseItem.itemData.events == null || _baseItem.itemData.events.Count == 0)
        {
            Debug.Log($"[EventTiming] {_baseItem.itemData.name}: không có event nào được gán trong ItemData");
            return;
        }
        if (SceneManager.instance == null) return;
        if (TimeManager.instance == null) return;

        // Chọn ngẫu nhiên 1 event từ danh sách của tòa nhà
        var evt = _baseItem.itemData.events[UnityEngine.Random.Range(0, _baseItem.itemData.events.Count)];
        if (evt == null) return;

        // Tung xúc xắc
        float roll = UnityEngine.Random.Range(0f, 1f);
        Debug.Log($"[EventTiming] {_baseItem.itemData.name}: rolling event '{evt.eventName}' | roll={roll:F2} | prob={evt.triggerProbability:F2} | kết quả={(roll <= evt.triggerProbability ? "Sẽ xảy ra" : "Không xảy ra")}");

        if (roll <= evt.triggerProbability)
        {
            _pendingEvent = evt;
            float timeRemaining = TimeManager.instance.timeRemaining;
            float maxTriggerTime = timeRemaining - 10f; // cooldown 10s trước cuối kỳ
            if (maxTriggerTime > 0)
            {
                _nextEventTime = UnityEngine.Random.Range(0f, maxTriggerTime);
                Debug.Log($"[EventTiming] {_baseItem.itemData.name}: event sẽ hiện khi timeRemaining <= {_nextEventTime:F1} (hiện tại={timeRemaining:F1})");
            }
            else
            {
                Debug.Log($"[EventTiming] {_baseItem.itemData.name}: timeRemaining ({timeRemaining:F1}) quá ngắn, không đủ thời gian spawn event");
            }
        }
    }

    /// <summary>
    /// Gọi mỗi frame để theo dõi thời điểm xảy ra event và kiểm tra kỳ mới.
    /// </summary>
    private void UpdateEventTiming()
    {
        if (SceneManager.instance == null || TimeManager.instance == null || TimeManager.instance.isPaused) return;
        if (_baseItem == null || _baseItem.itemData == null) return;
        if (_baseItem.itemData.events == null || _baseItem.itemData.events.Count == 0) return;

        // Kiểm tra kỳ mới
        int currentSemester = SceneManager.instance.currentSemester;
        if (currentSemester != _lastEventSemester)
        {
            Debug.Log($"[EventTiming] {_baseItem.itemData.name}: kỳ mới bắt đầu (kỳ {currentSemester}), bắt đầu roll event...");
            _lastEventSemester = currentSemester;
            if (!readyForEvent)
                CalculateNextEventForBuilding();
        }

        // Kiểm tra thời điểm kích hoạt event
        if (_nextEventTime >= 0 && !readyForEvent && TimeManager.instance.timeRemaining <= _nextEventTime)
        {
            _nextEventTime = -1f;
            readyForEvent = true;
            Debug.Log($"[EventTiming] {_baseItem.itemData.name}: HIỆN EventIcon! event='{_pendingEvent?.eventName}'");
            // Hiển EventIcon, ghi đè lên icon resource nếu đang có
            _baseItem.UI.ShowCollectNotificationUI(true, "event");
        }
    }

    /// <summary>
    /// Gọi khi người chơi tap vào tòa nhà có EventIcon.
    /// Mở cửa sổ UniversityEvent.
    /// </summary>
    public void TriggerEvent()
    {
        if (_pendingEvent == null) return;
        if (UniversityEventManager.instance == null) return;
        UniversityEventManager.instance.ShowEventForBuilding(_pendingEvent, _baseItem);
    }

    /// <summary>
    /// Gọi sau khi người chơi chọn option trong EventResultOptionWindow.
    /// Xóa event, tắt EventIcon, hiển lại icon resource nếu có.
    /// </summary>
    public void ResolveEvent()
    {
        readyForEvent = false;
        _pendingEvent = null;
        _nextEventTime = -1f;

        if (readyForCollection && !string.IsNullOrEmpty(_currentResourceType))
        {
            // Tài nguyên đã ready nhưng bị EventIcon che — hiển lại icon resource
            _baseItem.UI.ShowCollectNotificationUI(true, _currentResourceType);
        }
        else
        {
            // Không có tài nguyên sẵn sàng — ẩn notification
            _baseItem.UI.ShowCollectNotificationUI(false, "");
        }
    }

    // ========================
    // SAVE / LOAD SUPPORT
    // ========================

    public double GetLastCollectedTime()
    {
        return _lastCollectedTime;
    }

    public void SetLastCollectedTime(double time)
    {
        _lastCollectedTime = time;
    }

    private double GetCurrentTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public string GetFormattedConstructionTime()
    {
        int minutes = Mathf.FloorToInt(constructionTimeRemaining / 60);
        int seconds = Mathf.FloorToInt(constructionTimeRemaining % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}