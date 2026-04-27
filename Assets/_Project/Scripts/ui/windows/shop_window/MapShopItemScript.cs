using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for individual items in Map Shop UI.
/// Similar to SubCategoryItemScript but directly uses item IDs.
/// </summary>
public class MapShopItemScript : MonoBehaviour
{
	/* references */
	public SubCategoryItemScript subCategoryItemScript;
	public Text Name;
	public Text PriceText;
	public Image Image;
	public GameObject LockImage;
	public Text UnlockText;
	public GameObject InfoButton;


	/* private variables */
	private int _itemId = 0;
	private ItemsCollection.ItemData _itemData;

	public void SetItemData(int itemId, ItemsCollection.ItemData itemData)
	{
		_itemId = itemId;
		_itemData = itemData;

		if (_itemData != null)
		{
			if (Name != null)
			{
				Name.text = _itemData.name;
			}

			if (PriceText != null)
			{
				PriceText.text = _itemData.configuration.price.ToString();
			}

			if (Image != null)
			{
				SetImageSprite(itemId);
			}

			if (UnlockText != null)
			{
				UnlockText.text = "Semester " + _itemData.configuration.unlockItemAtSemester;
			}

			bool isUnlocked = SceneManager.instance.currentSemester >= _itemData.configuration.unlockItemAtSemester;
			if (LockImage != null)
			{
				LockImage.SetActive(!isUnlocked);
			}

			if (InfoButton != null)
			{
				InfoButton.SetActive(isUnlocked);
			}
		}

	}

	private void SetImageSprite(int itemId)
	{
		if (subCategoryItemScript == null) return;

		switch (itemId)
		{
			case 3635: // D4
				Image.sprite = subCategoryItemScript.D4Sprite;
				break;
			case 8216: // C2
				Image.sprite = subCategoryItemScript.C2Sprite;
				break;
			case 2454: // C3
				Image.sprite = subCategoryItemScript.C3Sprite;
				break;
			case 5835: // C3B
				Image.sprite = subCategoryItemScript.C3BSprite;
				break;
			case 3265: // C4
				Image.sprite = subCategoryItemScript.C4Sprite;
				break;
			case 3504: // C5
				Image.sprite = subCategoryItemScript.C5Sprite;
				break;
			case 2617: // C6
				Image.sprite = subCategoryItemScript.C6Sprite;
				break;
			case 3336: // C7
				Image.sprite = subCategoryItemScript.C7Sprite;
				break;
			case 9295: // C9
				Image.sprite = subCategoryItemScript.C9Sprite;
				break;
			case 8385: // C10
				Image.sprite = subCategoryItemScript.C10Sprite;
				break;
			case 4407: // D35
				Image.sprite = subCategoryItemScript.D35Sprite;
				break;
			case 6330: // D6
				Image.sprite = subCategoryItemScript.D6Sprite;
				break;
			case 5134: // D8
				Image.sprite = subCategoryItemScript.D8Sprite;
				break;
			// case 5342: // B8
			// 	Image.sprite = subCategoryItemScript.B8Sprite;
			// 	break;
			case 1399: // Canteen
				Image.sprite = subCategoryItemScript.CanteenSprite;
				break;
			case 4132: // GaraD6
				Image.sprite = subCategoryItemScript.GaraD6Sprite;
				break;
			case 2496: // C1
				Image.sprite = subCategoryItemScript.C1Sprite;
				break;
			case 2949: // GIAI_PHONG_GATE
				Image.sprite = subCategoryItemScript.GiaiPhongGateSprite;
				break;
			case 1251: // TDN_GATE
				Image.sprite = subCategoryItemScript.TDNGateSprite;
				break;
			case 6677: // LIBRARY
				Image.sprite = subCategoryItemScript.LibSprite;
				break;
			case 7666: // WALL
				Image.sprite = subCategoryItemScript.WallSprite;
				break;
			case 5341: // TREE3
				Image.sprite = subCategoryItemScript.Tree3Sprite;
				break;
			case 3090: // ITIMS
				Image.sprite = subCategoryItemScript.ITIMSSprite;
				break;
			case 1628: // Security Room
				Image.sprite = subCategoryItemScript.SecurityRoomSprite;
				break;
			case 9138: // PC Lab
				Image.sprite = subCategoryItemScript.PCLabSprite;
				break;
		}
	}

	public void OnClick()
	{
		if (_itemData == null)
		{
			return;
		}

		bool isUnlocked = SceneManager.instance.currentSemester >= _itemData.configuration.unlockItemAtSemester;
		if (!isUnlocked)
		{
			Debug.Log("Item is locked! Requires Semester " + _itemData.configuration.unlockItemAtSemester);
			return;
		}


		int price = _itemData.configuration.price;
		string resource = _itemData.configuration.resourceType;

		// Try to consume resources
		bool canBuild = SceneManager.instance.HasEnoughResource(resource, price);

		if (!canBuild)
		{
			Debug.Log("Not enough resource: " + resource);
			return;
		}

		// Create the item in preview mode
		BaseItemScript item = SceneManager.instance.AddItem(_itemId, false, true, 1, true);

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

		ItemWindowScript itemWindow = this.GetComponentInParent<ItemWindowScript>();
		if (itemWindow != null)
		{
			MapShopAreaScript mapShop = itemWindow.GetMapShopArea();
			if (mapShop != null)
			{
				mapShop.RemoveItem(_itemId);
				if (mapShop.GetItemIds().Count == 0)
				{
					mapShop.gameObject.SetActive(false);
				}
			}
			itemWindow.Close();
		}
	}

	public void OnClickInfoButton()
	{
		if (_itemData != null)
		{
			InfoWindowScript infoWindow = UIManager.instance.ShowInfoWindow();
			infoWindow.Init(_itemData);
		}
	}
}
