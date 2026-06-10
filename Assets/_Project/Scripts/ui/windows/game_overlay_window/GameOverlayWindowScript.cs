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
	public ProgressPanelScript LevelInfo;
	public Text LevelLabel;
	public Text SemesterText;
	public ProgressPanelScript TimeInfo;

	public GameObject ZoomInButton;
	public GameObject ZoomOutButton;
	public GameObject ShopButton;
	public GameObject MissionButton;
	public GameObject CollectionButton;
	public GameObject HandTutorial;
	public GameObject HandTutMission;
	public GameObject TutorialSemester;
	public GameObject ContinueButton;
	public GameObject Hint;
	public GameObject CollectionHint;
	public Animator anim;
	public GameObject TutorialTime;
	public GameObject TutorialCollection;
	public GameObject HappyWarning;
	public GameObject EduWarning;
	public Text HappyHintText;
	public Text EduHintText;

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
		if (this.HappyWarning != null) this.HappyWarning.SetActive(SceneManager.instance.numberOfHappyInStorage <= 15);

		this.StudentInfo.hasMaxValue = true;
		this.StudentInfo.maxValue = SceneManager.instance.studentStorageCapacity;
		this.StudentInfo.value = SceneManager.instance.numberOfStudentInStorage;
		this.StudentInfo.showAsCurrentMax = true;  // Hiển thị dạng "5/10"

		this.EducationInfo.hasMaxValue = true;
		this.EducationInfo.maxValue = SceneManager.instance.educationStorageCapacity;
		this.EducationInfo.value = SceneManager.instance.numberOfEducationInStorage;
		this.EducationInfo.isPercent = true;
		if (this.EduWarning != null) this.EduWarning.SetActive(SceneManager.instance.numberOfEducationInStorage <= 15);

		if (this.HappyHintText != null && SceneManager.instance != null)
		{
			this.HappyHintText.text = $"Độ hạnh phúc: {(int)this.HappyInfo.value}/{SceneManager.instance.happyStorageCapacity}";
		}
		if (this.EduHintText != null && SceneManager.instance != null)
		{
			this.EduHintText.text = $"Độ học vấn: {(int)this.EducationInfo.value}/{SceneManager.instance.educationStorageCapacity}";
		}

		this.LevelInfo.hasMaxValue = true;
		this.LevelInfo.maxValue = 100;
		this.LevelInfo.value = SceneManager.instance.levelProgress;
		this.LevelInfo.isPercent = true;

		this.RefreshLevelUI();
		this.RefreshSemesterUI();

		// Logic Tutorial: Nếu level < 2, tắt các thông số và dừng thời gian
		if (SceneManager.instance.currentLevel < 2)
		{
			if (HappyInfo != null) HappyInfo.gameObject.SetActive(false);
			if (EducationInfo != null) EducationInfo.gameObject.SetActive(false);
			if (StudentInfo != null) StudentInfo.gameObject.SetActive(false);
			if (TimeInfo != null) TimeInfo.gameObject.SetActive(false);
			
			if (TimeManager.instance != null)
			{
				TimeManager.instance.SetPaused(true);
			}
		}

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

		if (TimeInfo != null && TimeManager.instance != null)
		{
			// Mặc định hiển thị đếm ngược học kỳ
			TimeInfo.hasMaxValue = true;
			TimeInfo.maxValue = TimeManager.instance.semesterDuration;
			TimeInfo.value = TimeManager.instance.timeRemaining;
			
			if (TimeInfo.ValueLabel != null)
			{
				TimeInfo.ValueLabel.text = TimeManager.instance.GetFormattedTime();
			}
		}

		if (this.HappyHintText != null && this.HappyInfo != null && SceneManager.instance != null)
		{
			this.HappyHintText.text = $"Độ hạnh phúc: {(int)this.HappyInfo.value}/{SceneManager.instance.happyStorageCapacity}";
		}

		if (this.EduHintText != null && this.EducationInfo != null && SceneManager.instance != null)
		{
			this.EduHintText.text = $"Độ học vấn: {(int)this.EducationInfo.value}/{SceneManager.instance.educationStorageCapacity}";
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

	public void ShowCollectionHint()
	{
		if (CollectionHint != null)
		{
			CollectionHint.SetActive(true);
		}
	}

	public void HideCollectionHint()
	{
		if (CollectionHint != null)
		{
			CollectionHint.SetActive(false);
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

	public void OnClickCollectionButton()
	{
		UIManager.instance.ShowCollectionWindowViewOnly();

		// Nếu đang trong TutorialCollection thì tắt nó và khôi phục giao diện
		if (TutorialCollection != null && TutorialCollection.activeSelf)
		{
			TutorialCollection.SetActive(false);
			
			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.Close();
			}

			// Bật lại các Object bình thường
			if (GoldInfo != null) GoldInfo.gameObject.SetActive(true);
			if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(true);
			if (StudentInfo != null) StudentInfo.gameObject.SetActive(true);
			if (TimeInfo != null) TimeInfo.gameObject.SetActive(true);
			
			if (ShopButton != null) ShopButton.SetActive(true);
			if (MissionButton != null) MissionButton.SetActive(true);
			
			// Zoom button (theo logic bình thường là bật ZoomOut)
			if (ZoomOutButton != null) ZoomOutButton.SetActive(true);
			if (ZoomInButton != null) ZoomInButton.SetActive(false);

			bool isLevel2 = SceneManager.instance != null && SceneManager.instance.currentLevel >= 2;
			if (HappyInfo != null) HappyInfo.gameObject.SetActive(isLevel2);
			if (EducationInfo != null) EducationInfo.gameObject.SetActive(isLevel2);
		}
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

	public void RefreshLevelUI()
	{
		if (this.LevelInfo != null)
			this.LevelInfo.TweenValueChange(SceneManager.instance.levelProgress);
			
		if (this.LevelLabel != null)
			this.LevelLabel.text = SceneManager.instance.currentLevel.ToString();
	}

	public void RefreshSemesterUI()
	{
		if (this.SemesterText != null)
			this.SemesterText.text = SceneManager.instance.currentSemester.ToString();
	}

	public void OnClickEndSemester()
	{
		// Kích hoạt tiến trình kết thúc kỳ chính thức trong SceneManager
		SceneManager.instance.CompleteSemester();
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
			if (HappyWarning != null) HappyWarning.SetActive(value <= 15);
		}
		else if (resourceType == "student")
		{
			StudentInfo.TweenValueChange(value);
		}
		else if (resourceType == "education")
		{
			EducationInfo.TweenValueChange(value);
			if (EduWarning != null) EduWarning.SetActive(value <= 15);
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
		bool isLevel2 = SceneManager.instance.currentLevel >= 2;
		bool hasFinishedTutorial = TimeManager.instance != null && TimeManager.instance.hasFinishedFinalTutorial;

		// Hide/Show info panels
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(!active);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(!active);
		
		// Các thông số này chỉ hiện khi đạt level 2 hoặc đã qua tutorial và không trong mode tutorial ban đầu
		bool showLevel2Panels = !active && (isLevel2 || hasFinishedTutorial);
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(showLevel2Panels);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(showLevel2Panels);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(showLevel2Panels);
		if (TimeInfo != null) TimeInfo.gameObject.SetActive(showLevel2Panels);
		
		if (LevelInfo != null) LevelInfo.gameObject.SetActive(!active);

		// Hide/Show other buttons
		if (ZoomInButton != null) ZoomInButton.SetActive(!active);
		if (ZoomOutButton != null) ZoomOutButton.SetActive(!active);
		if (MissionButton != null) MissionButton.SetActive(!active);
		if (CollectionButton != null) CollectionButton.SetActive(!active);
		
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
		if (TimeInfo != null) TimeInfo.gameObject.SetActive(!active);
		
		if (active)
		{
			// Khi active, bật LevelInfo và TutorialSemester trước
			if (LevelInfo != null) LevelInfo.gameObject.SetActive(true);
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
			if (LevelInfo != null) LevelInfo.gameObject.SetActive(true);
			if (TutorialSemester != null) TutorialSemester.SetActive(false);
			if (ContinueButton != null) ContinueButton.SetActive(false);

			if (MissionButton != null) MissionButton.SetActive(false);
			if (HandTutMission != null) HandTutMission.SetActive(false);
		}

		// Hide other buttons
		if (ZoomInButton != null) ZoomInButton.SetActive(!active);
		if (ZoomOutButton != null) ZoomOutButton.SetActive(!active);
		if (ShopButton != null) ShopButton.SetActive(!active);
		if (CollectionButton != null) CollectionButton.SetActive(!active);
		
		// Let RefreshHint handle the hint visibility based on mission status
		RefreshHint();
		
		// Ensure other hand is off
		if (HandTutorial != null) HandTutorial.SetActive(false);
	}

	public void OnClickContinueTutorialSemester()
	{
		if (TutorialSemester != null) TutorialSemester.SetActive(false);
		if (ContinueButton != null) ContinueButton.SetActive(false);
		if (LevelInfo != null) LevelInfo.gameObject.SetActive(false); // tắt LevelInfo
		
		// Bật MissionButton
		if (MissionButton != null) MissionButton.SetActive(true);
		if (HandTutMission != null) HandTutMission.SetActive(true);
		
		if (TutorialWindowScript.instance != null)
		{
			TutorialWindowScript.instance.SwitchTutorialObject(2); // Tắt Tut 2 bật Tut 3
		}

		RefreshHint();
	}

	public void OnReachLevel2()
	{
		// Hiện lại các thông số
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(true);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(true);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(true);
		if (TimeInfo != null) TimeInfo.gameObject.SetActive(true);
		
		// Note: Thời gian vẫn giữ Pause cho đến khi đóng hướng dẫn (OnClickCloseTutorialWindow)
	}

	public void TriggerTutorialAfterUnlock()
	{
		// Hiện lại Overlay và đưa lên trên cùng
		this.ShowOverlay();
		this.transform.SetAsLastSibling();

		// Bật hướng dẫn về thời gian
		if (TutorialTime != null) TutorialTime.SetActive(true);

		// Tắt các Info khác và Button trong lúc hiện TutorialTime
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(false);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(false);
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(false);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(false);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(false);
		if (ShopButton != null) ShopButton.SetActive(false);
		if (MissionButton != null) MissionButton.SetActive(false);
		if (CollectionButton != null) CollectionButton.SetActive(false);
		if (ZoomInButton != null) ZoomInButton.SetActive(false);
		if (ZoomOutButton != null) ZoomOutButton.SetActive(false);

		// Gọi Tutorial Window nhưng đảm bảo Overlay nằm trên nó
		if (UIManager.instance != null)
		{
			UIManager.instance.ShowTutorialWindow();
			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.ShowCharacter(false);
				TutorialWindowScript.instance.ShowBoxchat(false); 
			}
		}

		// Một lần nữa đảm bảo Overlay ở trên cùng sau khi Tutorial Window được tạo
		this.transform.SetAsLastSibling();
	}

	public void TriggerTutorialCollection()
	{
		// Dừng thời gian khi vào tutorial này
		if (TimeManager.instance != null)
		{
			TimeManager.instance.isTutorialTimeRunning = false;
			TimeManager.instance.SetPaused(true);
		}

		// Hiện lại Overlay và đưa lên trên cùng
		this.ShowOverlay();
		this.transform.SetAsLastSibling();

		// Bật hướng dẫn về Collection
		if (TutorialCollection != null) TutorialCollection.SetActive(true);

		// Tắt các Info khác và Button trong lúc hiện TutorialCollection
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(false);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(false);
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(false);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(false);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(false);
		if (ShopButton != null) ShopButton.SetActive(false);
		if (MissionButton != null) MissionButton.SetActive(false);
		if (ZoomInButton != null) ZoomInButton.SetActive(false);
		if (ZoomOutButton != null) ZoomOutButton.SetActive(false);
		if (TimeInfo != null) TimeInfo.gameObject.SetActive(false);

		// Đảm bảo CollectionButton vẫn hiển thị để có thể click
		if (CollectionButton != null) CollectionButton.SetActive(true);

		// Gọi Tutorial Window nhưng đảm bảo Overlay nằm trên nó
		if (UIManager.instance != null)
		{
			UIManager.instance.ShowTutorialWindow();
			if (TutorialWindowScript.instance != null)
			{
				TutorialWindowScript.instance.ShowCharacter(false);
				TutorialWindowScript.instance.ShowBoxchat(false); 
				TutorialWindowScript.instance.SwitchTutorialObject(-1);
				if (TutorialWindowScript.instance.ContinueButton != null)
				{
					TutorialWindowScript.instance.ContinueButton.gameObject.SetActive(false);
				}
			}
		}

		// Một lần nữa đảm bảo Overlay ở trên cùng sau khi Tutorial Window được tạo
		this.transform.SetAsLastSibling();
	}

	public void OnClickContinueTutorialTime()
	{
		if (TutorialWindowScript.instance != null)
		{
			TutorialWindowScript.instance.Close();
		}

		// Tắt TutorialTime
		if (TutorialTime != null) TutorialTime.SetActive(false);

		// Bật lại các Object bình thường (trừ Happy và Education)
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(true);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(true);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(true);
		if (TimeInfo != null) TimeInfo.gameObject.SetActive(true);
		
		if (ShopButton != null) ShopButton.SetActive(true);
		if (MissionButton != null) MissionButton.SetActive(true);
		if (CollectionButton != null) CollectionButton.SetActive(true);
		
		// Zoom button (theo logic bình thường là bật ZoomOut)
		if (ZoomOutButton != null) ZoomOutButton.SetActive(true);
		if (ZoomInButton != null) ZoomInButton.SetActive(false);

		// Happy và Education bật lên luôn khi đã hết TutorialTime
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(true);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(true);

		// Tiếp tục thời gian nhưng chưa kết thúc tutorial
		if (TimeManager.instance != null)
		{
			TimeManager.instance.isTutorialTimeRunning = true;
			TimeManager.instance.SetPaused(false);
		}
	}

	public void OnClickCloseTutorialWindow()
	{
		if (TutorialWindowScript.instance != null)
		{
			TutorialWindowScript.instance.Close();
		}

		// Tắt TutorialTime
		if (TutorialTime != null) TutorialTime.SetActive(false);

		// Bật lại các Object bình thường (trừ Happy và Education)
		if (GoldInfo != null) GoldInfo.gameObject.SetActive(true);
		if (DiamondInfo != null) DiamondInfo.gameObject.SetActive(true);
		if (StudentInfo != null) StudentInfo.gameObject.SetActive(true);
		if (TimeInfo != null) TimeInfo.gameObject.SetActive(true);
		
		if (ShopButton != null) ShopButton.SetActive(true);
		if (MissionButton != null) MissionButton.SetActive(true);
		if (CollectionButton != null) CollectionButton.SetActive(true);
		
		// Zoom button (theo logic bình thường là bật ZoomOut)
		if (ZoomOutButton != null) ZoomOutButton.SetActive(true);
		if (ZoomInButton != null) ZoomInButton.SetActive(false);

		// Happy và Education bật lên luôn khi đã hết tutorial
		if (HappyInfo != null) HappyInfo.gameObject.SetActive(true);
		if (EducationInfo != null) EducationInfo.gameObject.SetActive(true);

		// Cuối cùng mới bật lại thời gian
		if (TimeManager.instance != null)
		{
			TimeManager.instance.isTutorialTimeRunning = false;
			TimeManager.instance.hasFinishedFinalTutorial = true;
			TimeManager.instance.SaveTimer();
			TimeManager.instance.SetPaused(false);
		}
	}

	public void ToggleGameObject(GameObject target)
	{
		if (target != null)
		{
			target.SetActive(!target.activeSelf);
		}
	}
}
