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
	public Text Name;
	public Text PriceText;
	public Image Image;

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

		}
	}

	public void OnClick()
	{
		if (_itemData == null)
		{
			return;
		}

		int price = _itemData.configuration.price;
		string resource = _itemData.configuration.resourceType;

		// Try to consume resources
		bool canBuild = SceneManager.instance.ConsumeResource(resource, price);

		if (!canBuild)
		{
			Debug.Log("Not enough resource: " + resource);
			return;
		}

		// Create the item
		BaseItemScript item = SceneManager.instance.AddItem(_itemId, false, true);

		if (item != null)
		{
			DataBaseManager.instance.UpdateItemData(item);
			if (CameraManager.instance != null)
			{
				CameraManager.instance.FocusOnItem(item);
			}
		}

		ItemWindowScript itemWindow = this.GetComponentInParent<ItemWindowScript>();
		if (itemWindow != null)
		{
			itemWindow.Close();
		}
	}
}
