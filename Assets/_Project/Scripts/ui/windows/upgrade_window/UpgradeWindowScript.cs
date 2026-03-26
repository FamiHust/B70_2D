using UnityEngine;
using UnityEngine.UI;

public class UpgradeWindowScript : WindowScript
{
    public static UpgradeWindowScript instance;

    public Text Title;
    public Text LevelText;
    public Text CostText;
    public Button UpgradeButton;

    private BaseItemScript _targetItem;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this._targetItem = SceneManager.instance.selectedItem;
        this.UpdateUI();
    }

    public void UpdateUI()
    {
        if (_targetItem == null) return;

        Title.text = _targetItem.itemData.name;
        LevelText.text = "Level: " + (_targetItem.level + 1);
        CostText.text = _targetItem.GetUpgradeCost().ToString();
    }

    public void OnClickUpgradeButton()
    {
        if (_targetItem == null) return;

        int cost = _targetItem.GetUpgradeCost();
        if (SceneManager.instance.ConsumeResource("gold", cost))
        {
            // Set callback to apply upgrade when construction is done
            _targetItem.OnConstructionComplete = (item) =>
            {
                _targetItem.level++;
                DataBaseManager.instance.UpdateItemData(_targetItem);
                SceneManager.instance.UpdateStudentStorageCapacity();

                // Refresh Selection UI if it's currently active
                if (_targetItem.UI.selectionUIInstance != null)
                {
                    _targetItem.UI.selectionUIInstance.RefreshLevel(_targetItem.level);
                }

                Debug.Log("Upgrade Complete! New Level: " + _targetItem.level);
            };

            // Start construction without requiring a builder
            _targetItem.StartConstruction(null);

            // Close windows
            this.Close();
            UIManager.instance.HideItemOptions();
        }
        else
        {
            Debug.Log("Not enough gold to upgrade!");
        }
    }
}
