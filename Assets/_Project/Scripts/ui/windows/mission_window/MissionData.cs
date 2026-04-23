using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
	public int itemId;
	public int goldReward;
	public bool isClaimed;

	public MissionData(int itemId, int goldReward, bool isClaimed = false)
	{
		this.itemId = itemId;
		this.goldReward = goldReward;
		this.isClaimed = isClaimed;
	}
}
