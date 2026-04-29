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
	public GameObject ShopButton;
	public GameObject MissionButton;
	public GameObject HandTutorial;
	public GameObject HandTutMission;
	public GameObject TutorialSemester;
	public GameObject ContinueButton;
	public GameObject Hint;
	public Animator anim;

	private float _nextHintCheckTime;



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

		this.RefreshHint();
	}

	private void Update()
	{
		if (Time.time >= _nextHintCheckTime)
		{
			RefreshHint();
			_nextHintCheckTime = Time.time + 0.5f;
		}
	}

	public void RefreshHint()
	{
		if (this.Hint != null)
		{
			bool hasReady = MissionWindowScript.HasReadyToClaimMission();
			bool isMissionButtonActive = MissionButton != null && MissionButton.activeInHierarchy;
			
			this.Hint.SetActive(hasReady && isMissionButtonActive);
		}
	}


	public void OnClickShopButton()
	{
		UIManager.instance.ShowShopWidow();

		if (SceneManager.instance != null && SceneManager.instance.isTutorialActive)
		{
			if (ShopButton != null) ShopButton.SetActive(false);
			if (HandTutorial != null) HandTutorial.SetActive(false);

			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.Close();
			}
		}
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

	public void HideOverlay()
	{
		if (anim != null) anim.Play("Hide");
	}

	public void ShowOverlay()
	{
		if (anim != null) anim.Play("Show");
		
		// If tutorial is active, ensure the correct tutorial state is applied
		if (SceneManager.instance != null && SceneManager.instance.isTutorialActive)
		{
			// If we have a building, check if it's finished to show the mission tutorial
			if (SceneManager.instance.GetBuildingCount() == 1)
			{
				// Only show mission tutorial state if the building is actually finished
				if (MissionWindowScript.HasReadyToClaimMission())
				{
					SetMissionTutorialState(true);
				}
				else
				{
					// If not finished, we are still in the waiting phase or normal tutorial state
					SetTutorialState(true);
					// Hide hand if we are just waiting
					if (HandTutorial != null) HandTutorial.SetActive(false);
				}
			}
			else
			{
				SetTutorialState(true);
			}
		}
	}

	public void SetTutorialState(bool active)
	{
		// Hide/Show info panels
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(!active);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(!active);
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(!active);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(!active);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(!active);
		if (SemesterInfo != null) SemesterInfo.gameObject.SetActive(!active);

		// Hide/Show other buttons
		if (ZoomInButton != null) ZoomInButton.SetActive(!active);
		if (ZoomOutButton != null) ZoomOutButton.SetActive(!active);
		if (MissionButton != null) MissionButton.SetActive(!active);
		
		// Let RefreshHint handle the hint visibility based on mission status
		RefreshHint();

		// ShopButton should be visible in both normal and tutorial states
		if (ShopButton != null) ShopButton.SetActive(true);
		
		// HandTutorial is only visible during the tutorial step
		if (HandTutorial != null) HandTutorial.SetActive(active);
		if (HandTutMission != null) HandTutMission.SetActive(false);
	}

	public void SetMissionTutorialState(bool active)
	{
		// Hide info panels
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(!active);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(!active);
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(!active);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(!active);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(!active);
		
		if (active)
		{
			// Khi active, bật SemesterInfo và TutorialSemester trước
			if (SemesterInfo != null) SemesterInfo.gameObject.SetActive(true);
			if (TutorialSemester != null) TutorialSemester.SetActive(true);
			if (ContinueButton != null) ContinueButton.SetActive(true);

			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.SwitchTutorialObject(1); // Tắt Tut 1 bật Tut 2
			}

			// Chưa bật MissionButton vội
			if (MissionButton != null) MissionButton.SetActive(false);
			if (HandTutMission != null) HandTutMission.SetActive(false);
		}
		else
		{
			if (SemesterInfo != null) SemesterInfo.gameObject.SetActive(true);
			if (TutorialSemester != null) TutorialSemester.SetActive(false);
			if (ContinueButton != null) ContinueButton.SetActive(false);

			if (MissionButton != null) MissionButton.SetActive(false);
			if (HandTutMission != null) HandTutMission.SetActive(false);
		}

		// Hide other buttons
		if (ZoomInButton != null) ZoomInButton.SetActive(!active);
		if (ZoomOutButton != null) ZoomOutButton.SetActive(!active);
		if (ShopButton != null) ShopButton.SetActive(!active);
		
		// Let RefreshHint handle the hint visibility based on mission status
		RefreshHint();
		
		// Ensure other hand is off
		if (HandTutorial != null) HandTutorial.SetActive(false);
	}

	public void OnClickContinueTutorialSemester()
	{
		if (TutorialSemester != null) TutorialSemester.SetActive(false);
		if (ContinueButton != null) ContinueButton.SetActive(false);
		if (SemesterInfo != null) SemesterInfo.gameObject.SetActive(false); // tắt SemesterInfo
		
		// Bật MissionButton
		if (MissionButton != null) MissionButton.SetActive(true);
		if (HandTutMission != null) HandTutMission.SetActive(true);
		
		if (TutorialWindowScript.instance != null)
		{
			TutorialWindowScript.instance.SwitchTutorialObject(2); // Tắt Tut 2 bật Tut 3
		}

		RefreshHint();
	}
}
