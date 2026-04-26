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

		MapShopAreaScript[] shopAreas = Object.FindObjectsOfType<MapShopAreaScript>();

		if (building != null)
		{
			CameraManager.instance.FocusOnItem(building, 10f);
			foreach (var area in shopAreas)
			{
				if (area.Arrow != null) area.Arrow.SetActive(false);
			}
		}
		else
		{
			// 2. If not found, look for MapShopArea that contains this item
			bool foundFocus = false;
			foreach (var area in shopAreas)
			{
				bool isTarget = area.itemIds.Contains(_itemId);
				if (area.Arrow != null)
				{
					area.Arrow.SetActive(isTarget);
				}

				if (isTarget && !foundFocus)
				{
					CameraManager.instance.FocusAndZoom(area.transform.position, 10f);
					foundFocus = true;
				}
			}
		}

		// Close the window
		MissionWindowScript.instance.Close();
	}

	public void OnClickComplete()
	{
		if (_data == null || _data.isClaimed) return;

		// End tutorial state before awarding resources to avoid errors with inactive UI panels
		if (SceneManager.instance != null && SceneManager.instance.isTutorialActive && SceneManager.instance.GetBuildingCount() == 1)
		{
			SceneManager.instance.SetMapShopAreasVisible(true);
			SceneManager.instance.isTutorialActive = false;

			if (GameOverlayWindowScript.instance != null)
			{
				GameOverlayWindowScript.instance.SetTutorialState(false);
			}

			// Also close the tutorial window
			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.Close();
			}
		}

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
		}

		// Award gold
		SceneManager.instance.CollectResource("gold", _goldReward);

		// Mark as claimed
		_data.isClaimed = true;
		DataBaseManager.instance.MarkMissionAsClaimed(_itemId);

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
