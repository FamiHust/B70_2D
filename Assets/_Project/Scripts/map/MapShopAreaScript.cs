using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script để attach vào GameObject trên map
/// Định nghĩa shop area với danh sách item có sẵn
/// Khi click, hiển thị ItemWindowScript với các item này
/// </summary>
public class MapShopAreaScript : MonoBehaviour
{
	public string areaName = "Map Shop";
	public List<int> itemIds = new List<int>();

	private Collider _collider;

	void Awake()
	{
		_collider = GetComponent<Collider>();

		if (_collider == null)
		{
			Debug.LogWarning($"MapShopAreaScript on {gameObject.name} requires a Collider component!");
		}
	}

	public void OnMapShopClicked()
	{
		if (itemIds.Count == 0)
		{
			Debug.LogWarning($"MapShopArea '{areaName}' has no items!");
			return;
		}

		// hiển thị ItemWindow thông qua UIManager, truyền reference của shop area này
		UIManager.instance.ShowMapShopWindow(areaName, itemIds, this);
	}

	public void AddItem(int itemId)
	{
		if (!itemIds.Contains(itemId))
		{
			itemIds.Add(itemId);
		}
	}

	public void RemoveItem(int itemId)
	{
		itemIds.Remove(itemId);
	}

	public void ClearItems()
	{
		itemIds.Clear();
	}

	public List<int> GetItemIds()
	{
		return new List<int>(itemIds);
	}
}
