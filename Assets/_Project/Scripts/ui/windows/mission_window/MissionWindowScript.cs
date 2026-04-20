using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWindowScript : WindowScript
{
	public static MissionWindowScript instance;

	/* prefabs */
	public GameObject ItemMissionPrefab;

	/* references */
	public Transform MissionListContainer;

	/* mission data */
	// Static list so it persists across window opens within the same session
	private static List<MissionData> _activeMissions;

	void Awake()
	{
		instance = this;

		if (_activeMissions == null)
		{
			InitDefaultMissions();
		}
	}

	void Start()
	{
		RenderMissions();
	}

	private void InitDefaultMissions()
	{
		_activeMissions = new List<MissionData>();

		// Add some default missions based on Shop IDs
		_activeMissions.Add(new MissionData(2496, 1000)); // C1
		_activeMissions.Add(new MissionData(8216, 1500)); // C2
		_activeMissions.Add(new MissionData(2454, 2000)); // C3
		_activeMissions.Add(new MissionData(3635, 500));  // D4

		_activeMissions.Add(new MissionData(3265, 1000)); // C4
		_activeMissions.Add(new MissionData(5835, 1000)); // C3B
		_activeMissions.Add(new MissionData(3504, 1000)); // C5
		_activeMissions.Add(new MissionData(2617, 1000)); // C6
		_activeMissions.Add(new MissionData(9295, 1000)); // C9
		_activeMissions.Add(new MissionData(8385, 1000)); // C10
		_activeMissions.Add(new MissionData(4407, 1000)); // D35
		_activeMissions.Add(new MissionData(6330, 1000)); // D6
		_activeMissions.Add(new MissionData(5134, 1000)); // D8
		_activeMissions.Add(new MissionData(1399, 1000)); // Canteen
		_activeMissions.Add(new MissionData(4132, 1000)); // GaraD6
		_activeMissions.Add(new MissionData(6677, 1000)); // LIBRARY
														  // _activeMissions.Add(new MissionData(7666, 100));  // WALL
														  // _activeMissions.Add(new MissionData(2949, 1000)); // GIAI_PHONG_GATE
														  // _activeMissions.Add(new MissionData(1251, 1000)); // TDN_GATE
														  // _activeMissions.Add(new MissionData(5341, 100));  // TREE3
		_activeMissions.Add(new MissionData(3336, 1000)); // C7
		_activeMissions.Add(new MissionData(5342, 1000)); // B8
	}

	public void RenderMissions()
	{
		// Sắp xếp: item đã unlock lên trước, sau đó sắp xếp theo semester yêu cầu (như trong Shop)
		_activeMissions.Sort((a, b) =>
		{
			ItemsCollection.ItemData dataA = Items.GetItem(a.itemId);
			ItemsCollection.ItemData dataB = Items.GetItem(b.itemId);

			if (dataA == null || dataB == null) return 0;

			bool unlockedA = SceneManager.instance.currentSemester >= dataA.configuration.unlockItemAtSemester;
			bool unlockedB = SceneManager.instance.currentSemester >= dataB.configuration.unlockItemAtSemester;

			// Nếu trạng thái unlock khác nhau, cái nào unlock rồi thì lên trước (-1)
			if (unlockedA != unlockedB)
			{
				return unlockedA ? -1 : 1;
			}

			// Nếu cùng trạng thái (cùng khóa hoặc cùng mở), sắp xếp theo số kỳ yêu cầu
			return dataA.configuration.unlockItemAtSemester.CompareTo(dataB.configuration.unlockItemAtSemester);
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
}
