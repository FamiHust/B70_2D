using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubCategoryItemScript : MonoBehaviour
{
	/* prefabs */
	// public Sprite BarrackSprite;
	// public Sprite BoatSprite;
	public Sprite D4Sprite;
	public Sprite C2Sprite;
	public Sprite C3Sprite;
	public Sprite C3BSprite;
	public Sprite C5Sprite;
	public Sprite C6Sprite;
	public Sprite C9Sprite;
	public Sprite C10Sprite;
	public Sprite D35Sprite;
	public Sprite D6Sprite;
	public Sprite D8Sprite;
	// public Sprite CampSprite;
	// public Sprite CannonSprite;
	// public Sprite ElixirCollectorSprite;
	// public Sprite ElixirStorageSprite;
	public Sprite C7Sprite;
	public Sprite C4Sprite;
	public Sprite CanteenSprite;
	public Sprite GaraD6Sprite;
	// public Sprite GoldStorageSprite;
	// public Sprite TowerSprite;
	public Sprite C1Sprite;
	public Sprite GiaiPhongGateSprite;
	public Sprite TDNGateSprite;
	public Sprite LibSprite;
	public Sprite WallSprite;
	public Sprite Tree3Sprite;
	public Sprite ITIMSSprite;
	public Sprite SecurityRoomSprite;
	public Sprite PCLabSprite;
	public Sprite MoneyLakeSprite;
	public Sprite D9Sprite;
	public Sprite TTVDSprite;
	public Sprite AlumniSprite;

	/* references */
	public Text Name;
	public Text PriceText;
	public Image Image;
	public GameObject LockImage;
	public Text UnlockText;
	public GameObject InfoButton;



	/* private variables */
	private ShopWindowScript.SubCategory _subCategory;

	public void SetSubCategory(ShopWindowScript.SubCategory subCategory)
	{
		this._subCategory = subCategory;

		int itemId = GetItemId(subCategory);
		ItemsCollection.ItemData itemData = Items.GetItem(itemId);
		if (itemData != null && PriceText != null)
		{
			PriceText.text = itemData.configuration.price.ToString();
			
			if (UnlockText != null)
			{
				UnlockText.text = "SV cấp " + itemData.configuration.unlockItemAtSemester + " mở";
			}
			
			bool isUnlocked = SceneManager.instance.currentLevel >= itemData.configuration.unlockItemAtSemester;
			if (LockImage != null)
			{
				LockImage.SetActive(!isUnlocked);
			}

			if (InfoButton != null)
			{
				InfoButton.SetActive(isUnlocked);
			}
		}


		switch (this._subCategory)
		{
			// case ShopWindowScript.SubCategory.BARRACK:
			// 	this.Name.text = "BARRACK";
			// 	this.Image.sprite = this.BarrackSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.BOAT:
			// 	this.Name.text = "BOAT";
			// 	this.Image.sprite = this.BoatSprite;
			// 	break;

			case ShopWindowScript.SubCategory.D4:
				this.Name.text = "Tòa D4";
				this.Image.sprite = this.D4Sprite;
				break;
		case ShopWindowScript.SubCategory.C2:
			this.Name.text = "Tòa C2";
			this.Image.sprite = this.C2Sprite;
			break;

		case ShopWindowScript.SubCategory.C3:
			this.Name.text = "Tòa C3";
			this.Image.sprite = this.C3Sprite;
			break;

		case ShopWindowScript.SubCategory.C3B:
			this.Name.text = "Tòa C3B";
			this.Image.sprite = this.C3BSprite;
			break;

		case ShopWindowScript.SubCategory.C5:
			this.Name.text = "Tòa C5";
			this.Image.sprite = this.C5Sprite;
			break;

		case ShopWindowScript.SubCategory.C6:
			this.Name.text = "Tòa C6";
			this.Image.sprite = this.C6Sprite;
			break;

		case ShopWindowScript.SubCategory.C9:
			this.Name.text = "Tòa C9";
			this.Image.sprite = this.C9Sprite;
			break;

		case ShopWindowScript.SubCategory.C10:
			this.Name.text = "Tòa C10";
			this.Image.sprite = this.C10Sprite;
			break;

		case ShopWindowScript.SubCategory.D35:
			this.Name.text = "Tòa D3-5";
			this.Image.sprite = this.D35Sprite;
			break;

		case ShopWindowScript.SubCategory.D6:
			this.Name.text = "Tòa D6";
			this.Image.sprite = this.D6Sprite;
			break;

		case ShopWindowScript.SubCategory.D8:
			this.Name.text = "Tòa D8";
			this.Image.sprite = this.D8Sprite;
			break;
			// case ShopWindowScript.SubCategory.CAMP:
			// 	this.Name.text = "CAMP";
			// 	this.Image.sprite = this.CampSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.CANNON:
			// 	this.Name.text = "CANNON";
			// 	this.Image.sprite = this.CannonSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.ELIXIR_COLLECTOR:
			// 	this.Name.text = "ELIXIR COLLECTOR";
			// 	this.Image.sprite = this.ElixirCollectorSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.ELIXIR_STORAGE:
			// 	this.Name.text = "ELIXIR STORAGE";
			// 	this.Image.sprite = this.ElixirStorageSprite;
			// 	break;

			case ShopWindowScript.SubCategory.C7:
				this.Name.text = "Tòa C7";
				this.Image.sprite = this.C7Sprite;
				break;
			// case ShopWindowScript.SubCategory.B8:
			// 		this.Name.text = "B8";
			// 		this.Image.sprite = this.B8Sprite;
			// 		break;

			case ShopWindowScript.SubCategory.C4:
				this.Name.text = "Tòa C4";
				this.Image.sprite = this.C4Sprite;
				break;
		case ShopWindowScript.SubCategory.Canteen:
			this.Name.text = "Căng tin";
			this.Image.sprite = this.CanteenSprite;
			break;

		case ShopWindowScript.SubCategory.GaraD6:
			this.Name.text = "Gara D6";
			this.Image.sprite = this.GaraD6Sprite;
			break;
			// case ShopWindowScript.SubCategory.GOLD_STORAGE:
			// 	this.Name.text = "GOLD STORAGE";
			// 	this.Image.sprite = this.GoldStorageSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.TOWER:
			// 	this.Name.text = "TOWER";
			// 	this.Image.sprite = this.TowerSprite;
			// 	break;

			case ShopWindowScript.SubCategory.C1:
				this.Name.text = "Tòa C1";
				this.Image.sprite = this.C1Sprite;
				break;

			// case ShopWindowScript.SubCategory.GIAI_PHONG_GATE:
			// 	this.Name.text = "GP GATE";
			// 	this.Image.sprite = this.GiaiPhongGateSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.TDN_GATE:
			// 	this.Name.text = "TDN GATE";
			// 	this.Image.sprite = this.TDNGateSprite;
			// 	break;

			case ShopWindowScript.SubCategory.LIBRARY:
				this.Name.text = "Thư viện TQB";
				this.Image.sprite = this.LibSprite;
				break;

			// case ShopWindowScript.SubCategory.WALL:
			// 	this.Name.text = "WALL";
			// 	this.Image.sprite = this.WallSprite;
			// 	break;

			// case ShopWindowScript.SubCategory.TREE3:
			// 	this.Name.text = "TREE3";
			// 	this.Image.sprite = this.Tree3Sprite;
			// 	break;
			case ShopWindowScript.SubCategory.ITIMS:
				this.Name.text = "Tòa ITIMS";
				this.Image.sprite = this.ITIMSSprite;
				break;
			case ShopWindowScript.SubCategory.SECURITY_ROOM:
				this.Name.text = "Phòng bảo vệ";
				this.Image.sprite = this.SecurityRoomSprite;
				break;
			case ShopWindowScript.SubCategory.PC_LAB:
				this.Name.text = "PC LAB";
				this.Image.sprite = this.PCLabSprite;
				break;
			case ShopWindowScript.SubCategory.MONEY_LAKE:
				this.Name.text = "Hồ Tiền";
				this.Image.sprite = this.MoneyLakeSprite;
				break;
			case ShopWindowScript.SubCategory.D9:
				this.Name.text = "Tòa D9";
				this.Image.sprite = this.D9Sprite;
				break;
			case ShopWindowScript.SubCategory.TTVD:
				this.Name.text = "Tòa TTVD";
				this.Image.sprite = this.TTVDSprite;
				break;
			case ShopWindowScript.SubCategory.Alumni:
				this.Name.text = "Tòa Alumni";
				this.Image.sprite = this.AlumniSprite;
				break;
		}
	}

	public void OnClick()
	{
		// LẤY DATA ITEM
		int itemIdToBuy = GetItemId(this._subCategory);
		ItemsCollection.ItemData itemToBuyData = Items.GetItem(itemIdToBuy);
		
		if (itemToBuyData != null)
		{
			bool isUnlocked = SceneManager.instance.currentLevel >= itemToBuyData.configuration.unlockItemAtSemester;
			if (!isUnlocked)
			{
				return;
			}
		}

		int itemId = 0;


		switch (this._subCategory)
		{
			case ShopWindowScript.SubCategory.D4:
				itemId = 3635;
				break;
			case ShopWindowScript.SubCategory.C2:
				itemId = 8216;
				break;
			case ShopWindowScript.SubCategory.C3:
				itemId = 2454;
				break;
			case ShopWindowScript.SubCategory.C3B:
				itemId = 5835;
				break;
			case ShopWindowScript.SubCategory.C5:
				itemId = 3504;
				break;
			case ShopWindowScript.SubCategory.C6:
				itemId = 2617;
				break;
			case ShopWindowScript.SubCategory.C9:
				itemId = 9295;
				break;
			case ShopWindowScript.SubCategory.C10:
				itemId = 8385;
				break;
			case ShopWindowScript.SubCategory.D35:
				itemId = 4407;
				break;
			case ShopWindowScript.SubCategory.D6:
				itemId = 6330;
				break;
			case ShopWindowScript.SubCategory.D8:
				itemId = 5134;
				break;
			case ShopWindowScript.SubCategory.C4:
				itemId = 3265;
				break;
			case ShopWindowScript.SubCategory.Canteen:
				itemId = 1399;
				break;
			case ShopWindowScript.SubCategory.GaraD6:
				itemId = 4132;
				break;
			case ShopWindowScript.SubCategory.C1:
				itemId = 2496;
				break;
			case ShopWindowScript.SubCategory.LIBRARY:
				itemId = 6677;
				break;
			case ShopWindowScript.SubCategory.C7:
				itemId = 3336;
				break;
			case ShopWindowScript.SubCategory.ITIMS:
				itemId = 3090;
				break;
			case ShopWindowScript.SubCategory.SECURITY_ROOM:
				itemId = 1628;
				break;
			case ShopWindowScript.SubCategory.PC_LAB:
				itemId = 9138;
					break;
			case ShopWindowScript.SubCategory.MONEY_LAKE:
				itemId = 9242;
				break;
			case ShopWindowScript.SubCategory.D9:
				itemId = 9818;
				break;
			case ShopWindowScript.SubCategory.TTVD:
				itemId = 3702;
				break;
			case ShopWindowScript.SubCategory.Alumni:
				itemId = 8099;
				break;
		}

		// LẤY DATA ITEM
		ItemsCollection.ItemData itemData = Items.GetItem(itemId);

		int price = itemData.configuration.price;
		string resource = itemData.configuration.resourceType;

		bool canBuild = SceneManager.instance.HasEnoughResource(resource, price);

		if (!canBuild)
		{
			Debug.Log("Not enough resource");
			return;
		}

		// Create the item in preview mode
		BaseItemScript item = SceneManager.instance.AddItem(itemId, false, true, 1, true);

		if (item != null)
		{
			DataBaseManager.instance.UpdateItemData(item);
			
			// Focus and select the item to show Yes/No buttons
			SceneManager.instance.OnItemTap(new CameraManager.CameraEvent { baseItem = item });
			
			if (CameraManager.instance != null)
			{
				CameraManager.instance.FocusOnItem(item, 10f);
			}
		}

		// Đóng window - check cả ShopWindowScript và ItemWindowScript
		ShopWindowScript shopWindow = this.GetComponentInParent<ShopWindowScript>();
		if (shopWindow != null)
		{
			shopWindow.Close();
			return;
		}

		ItemWindowScript itemWindow = this.GetComponentInParent<ItemWindowScript>();
		if (itemWindow != null)
		{
			// Destroy MapShopArea GameObject khi mua từ map shop
			MapShopAreaScript mapShop = itemWindow.GetMapShopArea();
			if (mapShop != null)
			{
				mapShop.gameObject.SetActive(false);
			}
			itemWindow.Close();
		}
	}

	private int GetItemId(ShopWindowScript.SubCategory subCategory)
	{
		int itemId = 0;
		switch (subCategory)
		{
			case ShopWindowScript.SubCategory.D4:
				itemId = 3635;
				break;
			case ShopWindowScript.SubCategory.C2:
				itemId = 8216;
				break;
			case ShopWindowScript.SubCategory.C3:
				itemId = 2454;
				break;
			case ShopWindowScript.SubCategory.C3B:
				itemId = 5835;
				break;
			case ShopWindowScript.SubCategory.C5:
				itemId = 3504;
				break;
			case ShopWindowScript.SubCategory.C6:
				itemId = 2617;
				break;
			case ShopWindowScript.SubCategory.C9:
				itemId = 9295;
				break;
			case ShopWindowScript.SubCategory.C10:
				itemId = 8385;
				break;
			case ShopWindowScript.SubCategory.D35:
				itemId = 4407;
				break;
			case ShopWindowScript.SubCategory.D6:
				itemId = 6330;
				break;
			case ShopWindowScript.SubCategory.D8:
				itemId = 5134;
				break;
			case ShopWindowScript.SubCategory.C4:
				itemId = 3265;
				break;
			case ShopWindowScript.SubCategory.C1:
				itemId = 2496;
				break;
			case ShopWindowScript.SubCategory.LIBRARY:
				itemId = 6677;
				break;
			case ShopWindowScript.SubCategory.C7:
				itemId = 3336;
				break;
			case ShopWindowScript.SubCategory.Canteen:
				itemId = 1399;
				break;
			case ShopWindowScript.SubCategory.GaraD6:
				itemId = 4132;
				break;
			case ShopWindowScript.SubCategory.ITIMS:
				itemId = 3090;
				break;
			case ShopWindowScript.SubCategory.SECURITY_ROOM:
				itemId = 1628;
				break;
			case ShopWindowScript.SubCategory.PC_LAB:
				itemId = 9138;
				break;
			case ShopWindowScript.SubCategory.MONEY_LAKE:
				itemId = 9242;
				break;
			case ShopWindowScript.SubCategory.D9:
				itemId = 9818;
				break;
			case ShopWindowScript.SubCategory.TTVD:
				itemId = 3702;
				break;
			case ShopWindowScript.SubCategory.Alumni:
				itemId = 8099;
				break;
		}
		return itemId;
	}
	public void OnClickInfoButton()
	{
		int itemId = GetItemId(this._subCategory);
		ItemsCollection.ItemData itemData = Items.GetItem(itemId);
		if (itemData != null)
		{
			InfoWindowScript infoWindow = UIManager.instance.ShowInfoWindow();
			infoWindow.Init(itemData);
		}
	}
}

