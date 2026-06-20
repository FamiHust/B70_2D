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
	public GameObject Arrow;
	public GameObject LockIcon;

	private ItemsCollection.ItemData _cachedItemData = null;
	private bool _hasCached = false;

	private Collider _collider;

	void Awake()
	{
		_collider = GetComponent<Collider>();

		if (_collider == null)
		{
			Debug.LogWarning($"MapShopAreaScript on {gameObject.name} requires a Collider component!");
		}
	}

	void Start()
	{
		UpdateLockStatus();
	}

	void Update()
	{
		UpdateLockStatus();
	}

	public void UpdateLockStatus()
	{
		if (LockIcon == null) return;

		if (itemIds.Count > 0)
		{
			if (!_hasCached)
			{
				int itemId = itemIds[0];
				_cachedItemData = Items.GetItem(itemId);
				_hasCached = true;
			}

			if (_cachedItemData != null && SceneManager.instance != null)
			{
				bool isUnlocked = SceneManager.instance.currentLevel >= _cachedItemData.configuration.unlockItemAtSemester;
				LockIcon.SetActive(!isUnlocked);
			}
		}
		else
		{
			LockIcon.SetActive(false);
		}
	}

	public void OnMapShopClicked()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlaySFX(SoundData.SFX_Button_Click);
		}

		if (SceneManager.instance != null)
		{
			SceneManager.instance.HideAllShopAreaArrows();
		}

		if (itemIds.Count == 0)
		{
			Debug.LogWarning($"MapShopArea '{areaName}' has no items!");
			return;
		}

		// Thay vì hiển thị ItemWindow, hiển thị InfoWindow cho item đầu tiên
		int itemId = itemIds[0];
		ItemsCollection.ItemData itemData = Items.GetItem(itemId);
		if (itemData != null)
		{
			InfoWindowScript infoWindow = UIManager.instance.ShowInfoWindow();
			infoWindow.Init(itemData);
		}
		else
		{
			Debug.LogWarning($"ItemId {itemId} not found in Items database!");
		}
	}

	public void AddItem(int itemId)
	{
		if (!itemIds.Contains(itemId))
		{
			itemIds.Add(itemId);
			_hasCached = false;
		}
	}

	public void RemoveItem(int itemId)
	{
		if (itemIds.Remove(itemId))
		{
			_hasCached = false;
		}
	}

	public void ClearItems()
	{
		itemIds.Clear();
		_hasCached = false;
	}

	public List<int> GetItemIds()
	{
		return new List<int>(itemIds);
	}
}
