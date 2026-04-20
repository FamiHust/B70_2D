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
		// C1 (Hanoi University of Science and Technology Gate or similar)
		_activeMissions.Add(new MissionData(2496, 1000)); 
		// C2
		_activeMissions.Add(new MissionData(8216, 1500));
		// C3
		_activeMissions.Add(new MissionData(2454, 2000));
		// D4 (Builder Hut usually)
		_activeMissions.Add(new MissionData(3635, 500));
	}

	public void RenderMissions()
	{
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
