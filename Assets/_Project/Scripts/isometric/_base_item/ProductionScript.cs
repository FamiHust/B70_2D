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

    // ⚠️ Dùng Unix Time để lưu lâu dài (không bị reset khi restart game)
    private double _lastCollectedTime = 0;

    public void SetData(BaseItemScript baseItem, double lastCollectedTime = 0)
    {
        this._baseItem = baseItem;
        this._productionRate = baseItem.itemData.configuration.productionRate;
        this._productType = baseItem.itemData.configuration.product;
        this._productPrice = baseItem.itemData.configuration.productPrice;

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

        // Cập nhật sức chứa sinh viên và tiến độ học kỳ ngay khi tòa hoàn thành xây dựng
        if (SceneManager.instance != null)
        {
            SceneManager.instance.UpdateStudentStorageCapacity();  // tăng cap AFTER xây xong
            SceneManager.instance.UpdateSemesterProgress();
        }
    }

    public void UpdateProduction()
    {
        if (this.isUnderConstruction)
            return;

        double time = GetCurrentTime() - this._lastCollectedTime;
        int productAmount = (int)((time / 3600.0) * this._productionRate);

        if (productAmount >= 1 && !readyForCollection)
        {
            readyForCollection = true;
            this._baseItem.UI.ShowCollectNotificationUI(true, this._productType);
        }
    }

    public void Collect()
    {
        double time = GetCurrentTime() - this._lastCollectedTime;
        int productAmount = (int)((time / 3600.0) * this._productionRate);

        if (productAmount > 0)
        {
            this._baseItem.Particles.ShowCollectionParticle(this._productType);
            this._baseItem.UI.ShowCollectNotificationUI(false, this._productType);

            this._lastCollectedTime = GetCurrentTime();
            this.readyForCollection = false;

            DataBaseManager.instance.UpdateItemData(this._baseItem);

            SceneManager.instance.CollectResource(this._productType, this._productPrice);

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
}