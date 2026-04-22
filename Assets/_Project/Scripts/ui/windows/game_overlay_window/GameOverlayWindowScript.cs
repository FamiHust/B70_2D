using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlayWindowScript : WindowScript
{

	public static GameOverlayWindowScript instance;

	/* object references */
	public ProgressPanelScript GoldInfo;
	// public ProgressPanelScript ElixirInfo;
	public ProgressPanelScript DiamondInfo;
	public ProgressPanelScript HappyInfo;
	public ProgressPanelScript StudentInfo;
	public ProgressPanelScript EducationInfo;
	public ProgressPanelScript SemesterInfo;
	public Text SemesterLabel;

	public GameObject ZoomInButton;
	public GameObject ZoomOutButton;



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

		this.HappyInfo.hasMaxValue = true;
		this.HappyInfo.maxValue = SceneManager.instance.happyStorageCapacity;
		this.HappyInfo.value = SceneManager.instance.numberOfHappyInStorage;
		this.HappyInfo.isPercent = true;

		this.StudentInfo.hasMaxValue = true;
		this.StudentInfo.maxValue = SceneManager.instance.studentStorageCapacity;
		this.StudentInfo.value = SceneManager.instance.numberOfStudentInStorage;
		this.StudentInfo.showAsCurrentMax = true;  // Hiển thị dạng "5/10"

		this.EducationInfo.hasMaxValue = true;
		this.EducationInfo.maxValue = SceneManager.instance.educationStorageCapacity;
		this.EducationInfo.value = SceneManager.instance.numberOfEducationInStorage;
		this.EducationInfo.isPercent = true;

		this.SemesterInfo.hasMaxValue = true;
		this.SemesterInfo.maxValue = 100;
		this.SemesterInfo.value = SceneManager.instance.semesterProgress;
		this.SemesterInfo.isPercent = true;

		this.RefreshSemesterUI();

		// Initial zoom button states
		if (this.ZoomInButton != null) this.ZoomInButton.SetActive(false);
		if (this.ZoomOutButton != null) this.ZoomOutButton.SetActive(true);
	}


	public void OnClickShopButton()
	{
		UIManager.instance.ShowShopWidow();
	}

	public void OnClickAttackButton()
	{
		SceneManager.instance.EnterAttackMode();
	}

	public void OnClickMissionButton()
	{
		UIManager.instance.ShowMissionWindow();
	}

	public void OnClickIncreaseStudent()
	{
		SceneManager.instance.numberOfStudentInStorage++;
		if (SceneManager.instance.numberOfStudentInStorage > SceneManager.instance.studentStorageCapacity)
		{
			SceneManager.instance.numberOfStudentInStorage = SceneManager.instance.studentStorageCapacity;
		}
		SceneManager.instance.SaveResources();
		SceneManager.instance.RefreshResourceUIs("student");
	}

	public void OnClickDecreaseStudent()
	{
		SceneManager.instance.numberOfStudentInStorage--;
		if (SceneManager.instance.numberOfStudentInStorage < 0)
		{
			SceneManager.instance.numberOfStudentInStorage = 0;
		}
		SceneManager.instance.SaveResources();
		SceneManager.instance.RefreshResourceUIs("student");
	}

	public void RefreshSemesterUI()
	{
		this.SemesterInfo.TweenValueChange(SceneManager.instance.semesterProgress);
		this.SemesterLabel.text = SceneManager.instance.currentSemester.ToString();
	}

	public void OnClickEndSemester()
	{
		// Tăng kỳ và reset progress
		SceneManager.instance.currentSemester++;
		SceneManager.instance.semesterProgress = 0;
		SceneManager.instance.SaveResources();

		this.RefreshSemesterUI();

		// Hiển thị window kỳ mới
		UIManager.instance.ShowNewSemesterWindow();
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
		else if (resourceType == "happy")
		{
			HappyInfo.TweenValueChange(value);
		}
		else if (resourceType == "student")
		{
			StudentInfo.TweenValueChange(value);
		}
		else if (resourceType == "education")
		{
			EducationInfo.TweenValueChange(value);
		}
	}

	public void OnClickZoomOut()
	{
		CameraManager.instance.ZoomOutAndLock();
		if (this.ZoomInButton != null) this.ZoomInButton.SetActive(true);
		if (this.ZoomOutButton != null) this.ZoomOutButton.SetActive(false);
	}

	public void OnClickZoomIn()
	{
		CameraManager.instance.ResetZoom(10f); // Default zoom 10
		if (this.ZoomInButton != null) this.ZoomInButton.SetActive(false);
		if (this.ZoomOutButton != null) this.ZoomOutButton.SetActive(true);
	}
}
