using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMissionScript : MonoBehaviour
{
	/* references */
	public Text MissionText;
	public Button CompleteButton;
	public Button UncompleteButton;

	/* private variables */
	private int _itemId;
	private int _goldReward;
	private MissionData _data;

	public void SetData(MissionData data)
	{
		this._data = data;
		this._itemId = data.itemId;
		this._goldReward = data.goldReward;

		ItemsCollection.ItemData itemData = Items.GetItem(_itemId);
		if (itemData != null)
		{
			this.MissionText.text = itemData.name.ToString();
		}
		else
		{
			this.MissionText.text = "Build Unknown Item (" + _itemId + ")";
		}

		UpdateStatus();
	}

	void Update()
	{
		// Optimize by only checking status every few frames or on specific events?
		// But for a simple UI, this is fine.
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (_data == null || _data.isClaimed) return;

		bool isFinished = SceneManager.instance.IsItemConstructionFinished(_itemId);
		
		if (CompleteButton != null)
			CompleteButton.gameObject.SetActive(isFinished);
		
		if (UncompleteButton != null)
			UncompleteButton.gameObject.SetActive(!isFinished);
	}


	public void OnClickUncomplete()
	{
		// 1. Try to find building in scene (placed but maybe under construction)
		BaseItemScript building = null;
		foreach (var item in SceneManager.instance.GetAllItems())
		{
			if (item.itemData.id == _itemId)
			{
				building = item;
				break;
			}
		}

		if (building != null)
		{
			CameraManager.instance.FocusOnItem(building, 10f);
		}
		else
		{
			// 2. If not found, look for MapShopArea that contains this item
			MapShopAreaScript[] shopAreas = Object.FindObjectsOfType<MapShopAreaScript>();
			foreach (var area in shopAreas)
			{
				if (area.itemIds.Contains(_itemId))
				{
					CameraManager.instance.FocusAndZoom(area.transform.position, 10f);
					break;
				}
			}
		}

		// Close the window
		MissionWindowScript.instance.Close();
	}

	public void OnClickComplete()
	{
		if (_data == null || _data.isClaimed) return;

		// Move camera to building and zoom to 8
		BaseItemScript building = null;
		foreach (var item in SceneManager.instance.GetAllItems())
		{
			if (item.itemData.id == _itemId)
			{
				building = item;
				break;
			}
		}

		if (building != null)
		{
			CameraManager.instance.FocusOnItem(building, 10f);

			// Close the window since we are focusing on world
			MissionWindowScript.instance.Close();
		}

		// Award gold
		SceneManager.instance.CollectResource("gold", _goldReward);
		
		// Increment semester progress
		SceneManager.instance.UpdateSemesterProgress();

		// Mark as claimed
		_data.isClaimed = true;

		// Refresh hint on overlay
		if (GameOverlayWindowScript.instance != null)
		{
			GameOverlayWindowScript.instance.RefreshHint();
		}

		// Play sound
		SoundManager.instance.PlaySound(SoundManager.instance.Tap2, false);

		// Remove from UI
		Destroy(this.gameObject);
	}
}
