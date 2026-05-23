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
	public RawImage Image;
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
				Image.texture = _itemData.thumb;
			}

			if (UnlockText != null)
			{
				UnlockText.text = "Level " + _itemData.configuration.unlockItemAtSemester;
			}

			bool isUnlocked = SceneManager.instance.currentLevel >= _itemData.configuration.unlockItemAtSemester;
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

	public void OnClick()
	{
		if (_itemData == null)
		{
			return;
		}

		bool isUnlocked = SceneManager.instance.currentLevel >= _itemData.configuration.unlockItemAtSemester;
		if (!isUnlocked)
		{
			Debug.Log("Item is locked! Requires Level " + _itemData.configuration.unlockItemAtSemester);
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
