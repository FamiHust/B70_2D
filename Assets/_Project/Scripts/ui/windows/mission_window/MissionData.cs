using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
	public int itemId;
	public int goldReward;
	public bool isClaimed = false;

	public MissionData(int itemId, int goldReward)
	{
		this.itemId = itemId;
		this.goldReward = goldReward;
		this.isClaimed = false;
	}
}
