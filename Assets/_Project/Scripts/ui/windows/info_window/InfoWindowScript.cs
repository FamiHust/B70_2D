using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindowScript : WindowScript
{
	public static InfoWindowScript instance;

	/* prefabs */
	public GameObject InfoItem;

	/* object references */
	public Text Title;
	public Text LevelText;
	public RawImage ThumbImage;
	public RawImage ThumbImageShadow;
	public GameObject InfoPanel;
	public Animator anim;
	public GameObject BuildButton;
	public GameObject UpgradeButton;
	public Text UnlockText;
	public GameObject LockPanel;

	[Header("Icons")]
	public Sprite BuildTimeIcon;
	public Sprite ProductionRateIcon;
	public Sprite ProductIcon;
	public Sprite DescriptionIcon;

	[Header("Tab Settings")]
	public Button InfoButton;
	public Button TeacherButton;
	public GameObject InfoTab;
	public GameObject TeacherTab;

	[Header("Teacher Tab References")]
	public Text TeacherName;
	public Text TeacherDescription;
	public Image TeacherImage;
	public GameObject RentButton;
	public GameObject SwitchTeacherButton;

	/* private vars */
	private BaseItemScript _baseItem;
	private ItemsCollection.ItemData _itemData;
	private Color _originalInfoColor = Color.white;
	private Color _originalTeacherColor = Color.white;

	void Awake()
	{
		instance = this;

		if (InfoButton != null && InfoButton.image != null)
		{
			_originalInfoColor = InfoButton.image.color;
		}

		if (TeacherButton != null && TeacherButton.image != null)
		{
			_originalTeacherColor = TeacherButton.image.color;
		}

		if (InfoButton != null)
		{
			InfoButton.onClick.AddListener(SelectInfoTab);
		}

		if (TeacherButton != null)
		{
			TeacherButton.onClick.AddListener(SelectTeacherTab);
		}

		if (RentButton != null)
		{
			Button btn = RentButton.GetComponent<Button>();
			if (btn != null)
			{
				btn.onClick.AddListener(OnClickRent);
			}
		}

		if (SwitchTeacherButton != null)
		{
			Button btn = SwitchTeacherButton.GetComponent<Button>();
			if (btn != null)
			{
				btn.onClick.AddListener(OnClickSwitchTeacher);
			}
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void Init(ItemsCollection.ItemData itemData, BaseItemScript baseItem = null)
	{
		this._itemData = itemData;
		this._baseItem = baseItem;

		if (TeacherButton != null)
		{
			bool isDecoration = itemData != null && ShopWindowScript.GetCategoryStringFromItemId(itemData.id) == "Trang trí";
			bool isNotBuiltYet = baseItem == null || baseItem.isUnderConstruction || baseItem.state == Common.State.PREVIEW;
			TeacherButton.gameObject.SetActive(!isDecoration && !isNotBuiltYet);
		}

		this.RenderInfo();
		this.SelectInfoTab();
	}

	public void RenderInfo()
	{
		if (this._itemData == null) return;

		// Clear previous items
		foreach (Transform child in this.InfoPanel.transform)
		{
			Destroy(child.gameObject);
		}

		this.Title.text = this._itemData.name;

		int currentLevel = this._baseItem != null ? this._baseItem.level : 1;
		if (this.LevelText != null)
		{
			this.LevelText.text = currentLevel.ToString();
		}

		this.ThumbImage.texture = this._itemData.thumb;
		if (this.ThumbImageShadow != null) this.ThumbImageShadow.texture = this._itemData.thumb;

		bool isCharacter = this._itemData.configuration.isCharacter;

		// if (!isCharacter)
		// {
		// 	//GRID SIZE
		// 	string gridSize = this._itemData.gridWidth.ToString() + "x" + this._itemData.gridHeight.ToString();
		// 	this._CreateInfoItem("Kích cỡ : ", gridSize);
		// }

		string buildTime = "Thời gian xây : " + this._itemData.configuration.buildTime.ToString() + "s";
		this._CreateInfoItem("Thời gian xây", buildTime, this.BuildTimeIcon);

		// if (this._itemData.configuration.speed > 0)
		// {
		// 	string speed = this._itemData.configuration.speed.ToString();
		// 	this._CreateInfoItem("Tốc độ : ", speed);
		// }

		// if (this._itemData.configuration.attackRange > 0)
		// {
		// 	string attackRange = this._itemData.configuration.attackRange.ToString();
		// 	this._CreateInfoItem("Tầm xa", attackRange);
		// }

		// if (this._itemData.configuration.defenceRange > 0)
		// {
		// 	string defenceRange = this._itemData.configuration.defenceRange.ToString();
		// 	this._CreateInfoItem("Defence Range", defenceRange);
		// }

		// if (this._itemData.configuration.hitPoints > 0)
		// {
		// 	string hitPoints = this._itemData.configuration.hitPoints.ToString();
		// 	this._CreateInfoItem("Hit Points", hitPoints);
		// }
  
		// if (this._itemData.configuration.productionRate > 0)
		// {
		// 	// string productionRate = ": " + this._itemData.configuration.productionRate.ToString();
		// 	// this._CreateInfoItem("Sản lượng", productionRate, this.ProductionRateIcon);

		// 	string product = ": " + this._itemData.configuration.product;
		// 	this._CreateInfoItem("Sản phẩm", product, this.ProductIcon);

		// 	int baseProductPrice = this._itemData.configuration.productPrice;
		// 	int effectiveProductPrice = baseProductPrice;
		// 	if (this._baseItem != null && this._baseItem.Production != null)
		// 	{
		// 		effectiveProductPrice = this._baseItem.Production.GetEffectiveProductPrice(this._itemData.configuration.product);
		// 	}
		// 	string productPrice = ": " + effectiveProductPrice.ToString();
		// 	this._CreateInfoItem("Giá sản phẩm", productPrice, this.ProductIcon);
		// }
		// if (this._baseItem != null)
		// {
		// 	this._CreateInfoItem("Cấp độ hiện tại : ", this._baseItem.level.ToString());
		// }

		if (!string.IsNullOrEmpty(this._itemData.description))
		{
			string description = this._itemData.description;
			this._CreateInfoItem("", description, this.DescriptionIcon);
		}

		if (this.BuildButton != null && this.UpgradeButton != null)
		{
			bool isUnlocked = SceneManager.instance.currentLevel >= this._itemData.configuration.unlockItemAtSemester;

			if (!isUnlocked && this._baseItem == null)
			{
				this.BuildButton.SetActive(false);
				this.UpgradeButton.SetActive(false);
				if (this.UnlockText != null)
				{
					this.UnlockText.gameObject.SetActive(true);
					this.UnlockText.text = "Yêu cầu SV cấp " + this._itemData.configuration.unlockItemAtSemester;
				}
				if (this.LockPanel != null) this.LockPanel.SetActive(true);
			}
			else
			{
				if (this.UnlockText != null) this.UnlockText.gameObject.SetActive(false);
				if (this.LockPanel != null) this.LockPanel.SetActive(false);

				if (this._baseItem == null)
				{
					this.BuildButton.SetActive(true);
					this.UpgradeButton.SetActive(false);
				}
				else
				{
					this.BuildButton.SetActive(false);
					if (this._baseItem.level < this._itemData.configuration.levelMax)
					{
						this.UpgradeButton.SetActive(true);
					}
					else
					{
						this.UpgradeButton.SetActive(false);
					}
				}
			}
		}

		this.RenderTeacherInfo();
	}

	private void RenderTeacherInfo()
	{
		if (this._baseItem != null && this._baseItem.assignedTeacher != null)
		{
			if (this.TeacherName != null) this.TeacherName.text = this._baseItem.assignedTeacher.teacherName;
			if (this.TeacherDescription != null) this.TeacherDescription.text = this._baseItem.assignedTeacher.skillDescription;
			if (this.TeacherImage != null)
			{
				this.TeacherImage.gameObject.SetActive(true);
				this.TeacherImage.sprite = this._baseItem.assignedTeacher.avatar;
			}
			if (this.RentButton != null) this.RentButton.SetActive(false);
			if (this.SwitchTeacherButton != null) this.SwitchTeacherButton.SetActive(true);
		}
		else
		{
			if (this.TeacherName != null) this.TeacherName.text = "Chưa có Giảng viên";
			if (this.TeacherDescription != null) this.TeacherDescription.text = "Tòa nhà này hiện chưa có giảng viên nào giảng dạy. Hãy thuê một giảng viên để nhận thêm các chỉ số buff.";
			if (this.TeacherImage != null)
			{
				this.TeacherImage.gameObject.SetActive(false);
			}
			if (this.RentButton != null)
			{
				this.RentButton.SetActive(this._baseItem != null);
			}
			if (this.SwitchTeacherButton != null) this.SwitchTeacherButton.SetActive(false);
		}
	}


	private void _CreateInfoItem(string property, string value, Sprite icon = null)
	{
		InfoItemCtrl comp = Utilities.CreateInstance(this.InfoItem, this.InfoPanel, true).GetComponent<InfoItemCtrl>();
		comp.SetData(property, value, icon);
	}

	public void HideWindow()
	{
		if (anim != null) anim.Play("Hide");
	}

	public void CloseWindow()
	{
		if (anim != null) anim.Play("Close");
	}

	public void ShowWindow()
	{
		if (anim != null) anim.Play("Show");
	}

	public void OnClickBuild()
	{
		if (_itemData == null) return;
		
		int itemId = _itemData.id;
		bool isUnlocked = SceneManager.instance.currentLevel >= _itemData.configuration.unlockItemAtSemester;
		if (!isUnlocked)
		{
			Debug.Log("Item is locked! Requires Level " + _itemData.configuration.unlockItemAtSemester);
			return;
		}

		int price = _itemData.configuration.price;
		string resource = _itemData.configuration.resourceType;

		bool canBuild = SceneManager.instance.HasEnoughResource(resource, price);

		if (!canBuild)
		{
			int currentAmount = resource == "diamond" ? SceneManager.instance.numberOfDiamondsInStorage : SceneManager.instance.numberOfGoldInStorage;
			int missingAmount = price - currentAmount;
			
			WarningWindow warningWindow = UIManager.instance.ShowWarningWindow();
			if (warningWindow != null)
			{
				if (resource == "diamond")
				{
					warningWindow.SetupDiamondWarning(missingAmount, currentAmount);
				}
				else
				{
					warningWindow.SetupGoldWarning(missingAmount, currentAmount);
				}
			}
			
			Debug.Log("Not enough resource: " + resource);
			return;
		}

		BaseItemScript item = SceneManager.instance.AddItem(itemId, false, true, 1, true);

		if (item != null)
		{
			DataBaseManager.instance.UpdateItemData(item);
			SceneManager.instance.OnItemTap(new CameraManager.CameraEvent { baseItem = item, isProgrammatic = true });
			if (CameraManager.instance != null)
			{
				CameraManager.instance.FocusOnItem(item, 10f);
			}
		}

		this.Close();
	}

	public void OnClickUpgrade()
	{
		if (_baseItem != null)
		{
			SceneManager.instance.selectedItem = _baseItem;
			UIManager.instance.ShowUpgradeWindow();
			this.Close();
		}
	}

	public void SelectInfoTab()
	{
		if (InfoTab != null) InfoTab.SetActive(true);
		if (TeacherTab != null) TeacherTab.SetActive(false);

		if (InfoButton != null && InfoButton.image != null)
		{
			InfoButton.image.color = _originalInfoColor;
		}

		if (TeacherButton != null && TeacherButton.image != null)
		{
			TeacherButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey
		}
	}

	public void SelectTeacherTab()
	{
		if (InfoTab != null) InfoTab.SetActive(false);
		if (TeacherTab != null) TeacherTab.SetActive(true);

		if (InfoButton != null && InfoButton.image != null)
		{
			InfoButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey
		}

		if (TeacherButton != null && TeacherButton.image != null)
		{
			TeacherButton.image.color = _originalTeacherColor;
		}
	}

	public void OnClickRent()
	{
		if (this._baseItem != null)
		{
			UIManager.instance.ShowCollectionWindowForAssign(this._baseItem);
		}
	}

	public void OnClickSwitchTeacher()
	{
		if (this._baseItem != null)
		{
			UIManager.instance.ShowCollectionWindowForSwitch(this._baseItem);
		}
	}
}
