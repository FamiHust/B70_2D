using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWindowScript : WindowScript
{
	public static MissionWindowScript instance;

	/* prefabs */
	public GameObject ItemMissionPrefab;

	public Transform MissionListContainer;

	public ProgressPanelScript GoldInfo;
	public ProgressPanelScript DiamondInfo;
	public ProgressPanelScript StudentInfo;
	public Animator anim;

	/* mission data */
	// Static list so it persists across window opens within the same session
	private static List<MissionData> _activeMissions;

	void Awake()
	{
		instance = this;

		if (_activeMissions == null)
		{
			InitActiveMissions();
		}
	}

	void Start()
	{
		RenderMissions();

		if (this.GoldInfo != null && SceneManager.instance != null)
		{
			this.GoldInfo.hasMaxValue = true;
			this.GoldInfo.maxValue = SceneManager.instance.goldStorageCapacity;
			this.GoldInfo.value = SceneManager.instance.numberOfGoldInStorage;
		}

		if (this.DiamondInfo != null && SceneManager.instance != null)
		{
			this.DiamondInfo.hasMaxValue = true;
			this.DiamondInfo.maxValue = SceneManager.instance.diamondStorageCapacity;
			this.DiamondInfo.value = SceneManager.instance.numberOfDiamondsInStorage;
		}

		if (this.StudentInfo != null && SceneManager.instance != null)
		{
			this.StudentInfo.hasMaxValue = true;
			this.StudentInfo.maxValue = SceneManager.instance.studentStorageCapacity;
			this.StudentInfo.value = SceneManager.instance.numberOfStudentInStorage;
			this.StudentInfo.showAsCurrentMax = true;
		}
	}

	public void UpdateResourcePanel()
	{
		if (this.GoldInfo != null && SceneManager.instance != null)
		{
			this.GoldInfo.TweenValueChange((float)SceneManager.instance.numberOfGoldInStorage);
		}

		if (this.DiamondInfo != null && SceneManager.instance != null)
		{
			this.DiamondInfo.TweenValueChange((float)SceneManager.instance.numberOfDiamondsInStorage);
		}

		if (this.StudentInfo != null && SceneManager.instance != null)
		{
			this.StudentInfo.TweenValueChange((float)SceneManager.instance.numberOfStudentInStorage);
		}
	}

	private static void InitActiveMissions()
	{
		_activeMissions = new List<MissionData>();

		// Lấy danh sách ID trực tiếp từ Shop để đảm bảo đồng bộ
		List<int> shopIds = ShopWindowScript.GetAllShopItemIds();
		List<int> claimedIds = DataBaseManager.instance.GetClaimedMissionIds();

		foreach (int itemId in shopIds)
		{
			// Tạo nhiệm vụ cho mỗi item trong shop, check xem đã nhận thưởng chưa
			bool isClaimed = claimedIds.Contains(itemId);
			_activeMissions.Add(new MissionData(itemId, 100, isClaimed));
		}
	}

	public void RenderMissions()
	{
		// Cache finished IDs for performance during sort
		HashSet<int> finishedIds = new HashSet<int>();
		if (SceneManager.instance != null)
		{
			foreach (var item in SceneManager.instance.GetAllItems())
			{
				// Building is finished if it has no progress UI (construction done)
				if (item.UI.progressUIInstance == null)
				{
					finishedIds.Add(item.itemData.id);
				}
			}
		}

		// Sắp xếp: 
		// 1. Tòa nào xây xong rồi thì cho lên trước để nhận thưởng
		// 2. Tiếp theo là theo trạng thái unlock (đã mở khóa > đang khóa)
		// 3. Tiếp theo là theo số kỳ yêu cầu (semester)
		// 4. Theo ID để đảm bảo thứ tự ổn định
		_activeMissions.Sort((a, b) =>
		{
			bool finishedA = finishedIds.Contains(a.itemId);
			bool finishedB = finishedIds.Contains(b.itemId);

			if (finishedA != finishedB)
			{
				return finishedA ? -1 : 1;
			}

			ItemsCollection.ItemData dataA = Items.GetItem(a.itemId);
			ItemsCollection.ItemData dataB = Items.GetItem(b.itemId);

			if (dataA == null || dataB == null) return 0;

			bool unlockedA = SceneManager.instance.currentSemester >= dataA.configuration.unlockItemAtSemester;
			bool unlockedB = SceneManager.instance.currentSemester >= dataB.configuration.unlockItemAtSemester;

			if (unlockedA != unlockedB)
			{
				return unlockedA ? -1 : 1;
			}

			// Nếu cùng trạng thái (cùng khóa hoặc cùng mở), sắp xếp theo số kỳ yêu cầu
			int semesterCompare = dataA.configuration.unlockItemAtSemester.CompareTo(dataB.configuration.unlockItemAtSemester);
			if (semesterCompare != 0)
			{
				return semesterCompare;
			}

			// Khóa phụ: Sắp xếp theo ID để đảm bảo tính ổn định (stable sort)
			return dataA.id.CompareTo(dataB.id);
		});

		// Clear existing items in container
		foreach (Transform child in MissionListContainer)
		{
			Destroy(child.gameObject);
		}

		// Create ItemMission for each non-claimed mission
		foreach (var mission in _activeMissions)
		{
			if (!mission.isClaimed)
			{
				GameObject inst = Utilities.CreateInstance(this.ItemMissionPrefab, this.MissionListContainer.gameObject, true);
				ItemMissionScript script = inst.GetComponent<ItemMissionScript>();
				if (script != null)
				{
					script.SetData(mission);
				}
			}
		}
	}

	public void Open()
	{
		this.gameObject.SetActive(true);
		RenderMissions();
	}

	public override void Close()
	{
		base.Close();
	}

	public static bool HasReadyToClaimMission()
	{
		if (_activeMissions == null)
		{
			InitActiveMissions();
		}

		if (_activeMissions == null || SceneManager.instance == null) return false;

		foreach (var mission in _activeMissions)
		{
			if (!mission.isClaimed && SceneManager.instance.IsItemConstructionFinished(mission.itemId))
			{
				return true;
			}
		}

		return false;
	}

	public void HideWindow()
	{
		if (anim != null) anim.Play("Hide");
	}

	public void ShowWindow()
	{
		if (anim != null) anim.Play("Show");
	}
}
