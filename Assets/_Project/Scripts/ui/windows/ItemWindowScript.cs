using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Window script for displaying items from Map Shop areas
/// Follows UIManager pattern - managed by UIManager singleton
/// </summary>
public class ItemWindowScript : WindowScript
{
	public static ItemWindowScript instance;

	/* prefabs */
	public GameObject SubCategoryItem;

	/* references */
	public ScrollRect ScrollView;
	public GameObject ItemsList;
	public GameObject BackButton;

	/* Map Shop references */
	private string _currentAreaName = "";
	private List<int> _currentItemIds = new List<int>();

	void Awake()
	{
		instance = this;
	}

	/* Map Shop area reference */
	private MapShopAreaScript _mapShopArea = null;

	/// <summary>
	/// Render items for Map Shop areas
	/// </summary>
	public void RenderItems(string areaName, List<int> itemIds, MapShopAreaScript mapShopArea = null)
	{
		_currentAreaName = areaName;
		_currentItemIds = new List<int>(itemIds);
		_mapShopArea = mapShopArea;

		this.ClearItemsList();

		// Create SubCategoryItem for each itemId
		for (int i = 0; i < itemIds.Count; i++)
		{
			int itemId = itemIds[i];
			ItemsCollection.ItemData itemData = Items.GetItem(itemId);
			
			if (itemData != null)
			{
				GameObject inst = Utilities.CreateInstance(this.SubCategoryItem, this.ItemsList, true);
				MapShopItemScript shopItem = inst.GetComponent<MapShopItemScript>();
				
				if (shopItem != null)
				{
					shopItem.SetItemData(itemId, itemData);
				}
				else
				{
					Debug.LogWarning($"SubCategoryItem prefab doesn't have MapShopItemScript component!");
				}
			}
			else
			{
				Debug.LogWarning($"ItemId {itemId} not found in Items database!");
			}
		}

		// Adjust layout for items count
		RectTransform rt = this.ItemsList.GetComponent<RectTransform>();
		Vector2 sizeDelta = this.ItemsList.GetComponent<RectTransform>().sizeDelta;
		GridLayoutGroup glg = this.ItemsList.GetComponent<GridLayoutGroup>();
		float spacing = glg != null ? glg.spacing.x : 0;
		sizeDelta.x = itemIds.Count * 250 + itemIds.Count * spacing;
		rt.sizeDelta = sizeDelta;

		this.ResetScrollPosition();
	}

	public void OnClickBackButton()
	{
		this.ClearItemsList();
		this.BackButton.SetActive(false);
		this.Close();
	}

	public void ClearItemsList()
	{
		foreach (Transform child in this.ItemsList.transform)
		{
			Destroy(child.gameObject);
		}
	}

	public void ResetScrollPosition()
	{
		if (this.ScrollView != null)
		{
			this.ScrollView.horizontalNormalizedPosition = 0.0f;
		}
	}

	public string GetCurrentAreaName()
	{
		return _currentAreaName;
	}

	public List<int> GetCurrentItemIds()
	{
		return _currentItemIds;
	}

	public MapShopAreaScript GetMapShopArea()
	{
		return _mapShopArea;
	}

	public override void Close()
	{
		_currentAreaName = "";
		_currentItemIds.Clear();
		_mapShopArea = null;
		base.Close();
	}
}
