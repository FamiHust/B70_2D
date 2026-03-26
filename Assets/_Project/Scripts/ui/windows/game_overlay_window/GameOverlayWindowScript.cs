using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlayWindowScript : WindowScript
{

	public static GameOverlayWindowScript instance;

	/* object references */
	public ProgressPanelScript GoldInfo;
	public ProgressPanelScript ElixirInfo;
	public ProgressPanelScript DiamondInfo;
	public ProgressPanelScript StudentInfo;

	private void Awake()
	{
		if (SceneManager.instance == null)
		{
			return;
		}

		instance = this;

	}

	private void Start()
	{
		this.GoldInfo.hasMaxValue = true;
		this.GoldInfo.maxValue = SceneManager.instance.goldStorageCapacity;
		this.GoldInfo.value = SceneManager.instance.numberOfGoldInStorage;

		// this.ElixirInfo.hasMaxValue = true;
		// this.ElixirInfo.maxValue = SceneManager.instance.elixirStorageCapacity;
		// this.ElixirInfo.value = SceneManager.instance.numberOfElixirInStorage;

		this.DiamondInfo.hasMaxValue = true;
		this.DiamondInfo.maxValue = SceneManager.instance.diamondStorageCapacity;
		this.DiamondInfo.value = SceneManager.instance.numberOfDiamondsInStorage;

		this.StudentInfo.hasMaxValue = true;
		this.StudentInfo.maxValue = SceneManager.instance.studentStorageCapacity;
		this.StudentInfo.value = SceneManager.instance.numberOfStudentInStorage;

	}

	public void OnClickShopButton()
	{
		UIManager.instance.ShowShopWidow();
	}

	public void OnClickAttackButton()
	{
		SceneManager.instance.EnterAttackMode();
	}

	//RESOURCE  COLLECTION
	public void CollectResource(string resourceType, int value)
	{

		if (resourceType == "gold")
		{
			GoldInfo.TweenValueChange(value);
		}
		else if (resourceType == "diamond")
		{
			DiamondInfo.TweenValueChange(value);
		}
		else if (resourceType == "student")
		{
			StudentInfo.TweenValueChange(value);
		}
	}
}
