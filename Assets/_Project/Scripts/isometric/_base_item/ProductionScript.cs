using System;
using UnityEngine;

public class ProductionScript : MonoBehaviour
{
    /* public vars */
    public bool readyForCollection = false;
    public int collectedAmount = 0;
    public bool isUnderConstruction = false;

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
                rate *= this._baseItem.assignedTeacher.influenceHappy;
            }
            else if (productType == "education")
            {
                rate *= this._baseItem.assignedTeacher.influenceEducation;
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
                price *= this._baseItem.assignedTeacher.influenceGold;
            }
            else if (productType == "happy")
            {
                price *= this._baseItem.assignedTeacher.influenceHappy;
            }
            else if (productType == "education")
            {
                price *= this._baseItem.assignedTeacher.influenceEducation;
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
            this._baseItem.UI.ShowCollectNotificationUI(true, firstReadyType);
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
                SceneManager.instance.CollectResource(pType, GetEffectiveProductPrice(pType));
                collectedAnything = true;
                if (string.IsNullOrEmpty(firstProduct)) firstProduct = pType;
            }
        }

        if (collectedAnything)
        {
            this._baseItem.Particles.ShowCollectionParticle(firstProduct);
            this._baseItem.UI.ShowCollectNotificationUI(false, firstProduct);

            this._lastCollectedTime = GetCurrentTime();
            this.readyForCollection = false;

            DataBaseManager.instance.UpdateItemData(this._baseItem);
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