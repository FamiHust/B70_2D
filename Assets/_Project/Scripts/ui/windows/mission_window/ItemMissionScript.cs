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
	public GameObject LockMission;
	public Image ProcessImage;

	/* private variables */
	private int _itemId;
	private int _goldReward;
	private MissionData _data;
	private BaseItemScript _cachedBuilding;

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

		if (LockMission != null)
		{
			ItemsCollection.ItemData itemData = Items.GetItem(_itemId);
			if (itemData != null)
			{
				LockMission.SetActive(itemData.configuration.unlockItemAtSemester > SceneManager.instance.currentSemester);
			}
		}

		bool isFinished = SceneManager.instance.IsItemConstructionFinished(_itemId);
		
		if (CompleteButton != null)
			CompleteButton.gameObject.SetActive(isFinished);
		
		if (UncompleteButton != null)
			UncompleteButton.gameObject.SetActive(!isFinished);

		if (ProcessImage != null)
		{
			if (_cachedBuilding == null || _cachedBuilding.isDestroyed)
			{
				_cachedBuilding = null;
				foreach (var item in SceneManager.instance.GetAllItems())
				{
					if (item.itemData.id == _itemId)
					{
						_cachedBuilding = item;
						break;
					}
				}
			}

			if (_cachedBuilding != null && _cachedBuilding.UI != null && _cachedBuilding.UI.progressUIInstance != null)
			{
				// Tòa nhà đang được xây
				ProcessImage.gameObject.SetActive(true);
				
				// Nếu ProcessImage có object cha là background (vd: bg tiến trình), ta có thể bật cha nó lên
				if (ProcessImage.transform.parent != null && ProcessImage.transform.parent.name.Contains("BG"))
				{
					ProcessImage.transform.parent.gameObject.SetActive(true);
				}

				float targetProgress = _cachedBuilding.UI.progressUIInstance.GetProgress();
				ProcessImage.fillAmount = Mathf.Lerp(ProcessImage.fillAmount, targetProgress, Time.deltaTime * 10f);
			}
			else
			{
				ProcessImage.gameObject.SetActive(false);
				ProcessImage.fillAmount = 0; // Reset fill amount khi tắt
				if (ProcessImage.transform.parent != null && ProcessImage.transform.parent.name.Contains("BG"))
				{
					ProcessImage.transform.parent.gameObject.SetActive(false);
				}
			}
		}
	}


	public void OnClickUncomplete()
	{
		// Khóa các mission khác khi đang trong tutorial của tòa nhà đầu tiên
		if (SceneManager.instance != null && SceneManager.instance.isTutorialActive && SceneManager.instance.GetBuildingCount() >= 1)
		{
			bool isForCurrentBuilding = false;
			foreach (var item in SceneManager.instance.GetAllItems())
			{
				if (item.itemData.id == _itemId)
				{
					isForCurrentBuilding = true;
					break;
				}
			}

			if (!isForCurrentBuilding)
			{
				bool isFirstBuildingClaimed = false;
				foreach (var item in SceneManager.instance.GetAllItems())
				{
					if (DataBaseManager.instance.GetClaimedMissionIds().Contains(item.itemData.id))
					{
						isFirstBuildingClaimed = true;
						break;
					}
				}

				if (isFirstBuildingClaimed)
				{
					// End tutorial state
					SceneManager.instance.SetMapShopAreasVisible(true);
					SceneManager.instance.isTutorialActive = false;

					if (GameOverlayWindowScript.instance != null)
					{
						GameOverlayWindowScript.instance.SetTutorialState(false);
					}

					// Close the tutorial window
					if (TutorialWindowScript.instance != null)
					{
						TutorialWindowScript.instance.Close();
					}

					// Bật lại chức năng của CloseButton của Mission Window
					if (MissionWindowScript.instance != null && MissionWindowScript.instance.CloseButton != null)
					{
						Button btn = MissionWindowScript.instance.CloseButton.GetComponent<Button>();
						if (btn != null)
						{
							btn.interactable = true;
						}
					}

					// Bật lại RayCastBlocker như cũ
					if (MissionWindowScript.instance != null && MissionWindowScript.instance.RayCastBlocker != null)
					{
						MissionWindowScript.instance.RayCastBlocker.SetActive(true);
					}
				}
				else
				{
					return;
				}
			}
		}

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
			// Đổi sang Tut tiếp theo
			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.SwitchTutorialObject(3); // Giả sử Tut 4 là index 3
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


		// Remove from UI
		Destroy(this.gameObject);
	}
}
