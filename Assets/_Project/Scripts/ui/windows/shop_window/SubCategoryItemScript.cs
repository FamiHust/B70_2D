using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubCategoryItemScript : MonoBehaviour
{
	/* references */
	public Text Name;
	public Text PriceText;
	public RawImage Image;
	public RawImage ImageShadow;
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
		if (itemData != null)
		{
			if (PriceText != null)
			{
				PriceText.text = itemData.configuration.price.ToString();
			}
			
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

			if (Image != null)
			{
				Image.texture = itemData.thumb;
			}
			if (ImageShadow != null)
			{
				ImageShadow.texture = itemData.thumb;
			}
		}


		switch (this._subCategory)
		{
			case ShopWindowScript.SubCategory.D4:
				this.Name.text = "Tòa D4";
				break;
			case ShopWindowScript.SubCategory.C2:
				this.Name.text = "Tòa C2";
				break;

			case ShopWindowScript.SubCategory.C3:
				this.Name.text = "Tòa C3";
				break;

			case ShopWindowScript.SubCategory.C3B:
				this.Name.text = "Tòa C3B";
				break;

			case ShopWindowScript.SubCategory.C5:
				this.Name.text = "Tòa C5";
				break;

			case ShopWindowScript.SubCategory.C6:
				this.Name.text = "Tòa C6";
				break;

			case ShopWindowScript.SubCategory.C9:
				this.Name.text = "Tòa C9";
				break;

			case ShopWindowScript.SubCategory.C10:
				this.Name.text = "Tòa C10";
				break;

			case ShopWindowScript.SubCategory.D35:
				this.Name.text = "Tòa D3-5";
				break;

			case ShopWindowScript.SubCategory.D6:
				this.Name.text = "Tòa D6";
				break;

			case ShopWindowScript.SubCategory.D7:
				this.Name.text = "Tòa D7";
				break;

			case ShopWindowScript.SubCategory.D8:
				this.Name.text = "Tòa D8";
				break;

			case ShopWindowScript.SubCategory.C7:
				this.Name.text = "Tòa C7";
				break;

			case ShopWindowScript.SubCategory.C4:
				this.Name.text = "Tòa C4";
				break;
			case ShopWindowScript.SubCategory.Canteen:
				this.Name.text = "Căng tin";
				break;

			case ShopWindowScript.SubCategory.GaraD6:
				this.Name.text = "Nhà xe D6";
				break;

			case ShopWindowScript.SubCategory.C1:
				this.Name.text = "Tòa C1";
				break;

			case ShopWindowScript.SubCategory.LIBRARY:
				this.Name.text = "Thư viện TQB";
				break;

			case ShopWindowScript.SubCategory.ITIMS:
				this.Name.text = "ITMS (C10B)";
				break;
			case ShopWindowScript.SubCategory.SECURITY_ROOM:
				this.Name.text = "Khu bảo vệ";
				break;
			case ShopWindowScript.SubCategory.PC_LAB:
				this.Name.text = "PTN Polime Compozit";
				break;
			case ShopWindowScript.SubCategory.MONEY_LAKE:
				this.Name.text = "Hồ Tiền";
				break;
			case ShopWindowScript.SubCategory.D9:
				this.Name.text = "Tòa D9";
				break;
			case ShopWindowScript.SubCategory.TTVD:
				this.Name.text = "Tòa TTVD";
				break;
			case ShopWindowScript.SubCategory.Alumni:
				this.Name.text = "Tòa Alumni";
				break;
			case ShopWindowScript.SubCategory.SAN_C2:
				this.Name.text = "Sân C2";
				break;
			case ShopWindowScript.SubCategory.ICEA:
				this.Name.text = "Tòa ICEA";
				break;
			case ShopWindowScript.SubCategory.KHUON_VIEN_C1:
				this.Name.text = "Khuôn viên C1";
				break;
			case ShopWindowScript.SubCategory.PARABOL_GATE:
				this.Name.text = "Cổng Parabol";
				break;
			case ShopWindowScript.SubCategory.TDN_GATE_3776:
				this.Name.text = "Cổng TĐN";
				break;
			case ShopWindowScript.SubCategory.DCV_GATE_1:
				this.Name.text = "Cổng ĐCV 1";
				break;
			case ShopWindowScript.SubCategory.DCV_GATE_2:
				this.Name.text = "Cổng ĐCV 2";
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
			case ShopWindowScript.SubCategory.D7:
				itemId = 8344;
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
			case ShopWindowScript.SubCategory.SAN_C2:
				itemId = 6437;
				break;
			case ShopWindowScript.SubCategory.ICEA:
				itemId = 4073;
				break;
			case ShopWindowScript.SubCategory.KHUON_VIEN_C1:
				itemId = 4563;
				break;
			case ShopWindowScript.SubCategory.PARABOL_GATE:
				itemId = 2403;
				break;
			case ShopWindowScript.SubCategory.TDN_GATE_3776:
				itemId = 3776;
				break;
			case ShopWindowScript.SubCategory.DCV_GATE_1:
				itemId = 1640;
				break;
			case ShopWindowScript.SubCategory.DCV_GATE_2:
				itemId = 9518;
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
			case ShopWindowScript.SubCategory.D7:
				itemId = 8344;
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
			case ShopWindowScript.SubCategory.SAN_C2:
				itemId = 6437;
				break;
			case ShopWindowScript.SubCategory.ICEA:
				itemId = 4073;
				break;
			case ShopWindowScript.SubCategory.KHUON_VIEN_C1:
				itemId = 4563;
				break;
			case ShopWindowScript.SubCategory.PARABOL_GATE:
				itemId = 2403;
				break;
			case ShopWindowScript.SubCategory.TDN_GATE_3776:
				itemId = 3776;
				break;
			case ShopWindowScript.SubCategory.DCV_GATE_1:
				itemId = 1640;
				break;
			case ShopWindowScript.SubCategory.DCV_GATE_2:
				itemId = 9518;
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

