using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemOptionsWindowScript : WindowScript
{

	public static ItemOptionsWindowScript instance;

	/* object references */
	public GameObject InfoButton;
	public GameObject UpgradeButton;
	public GameObject TrainButton;
	public GameObject BoostButton;
	public GameObject RemoveButton;
	public GameObject YesButton;
	public GameObject NoButton;
	public GameObject TeacherButton;
	public Text goldPriceText;
	public GameObject popupConfirm;
	public Animator confirmAnimator;

	private void Awake()
	{
		if (SceneManager.instance == null)
		{
			return;
		}

		instance = this;
		this.ShowOptions();
	}

	public void ShowOptions()
	{
		this.StartCoroutine(this._ShowOptions());
	}

	private float _waitTime = 0.08f;
	bool haveInfoButton = true;
	bool haveUpgradeButton = true;
	bool haveTrainButton = false;
	bool haveBoostButton = false;
	bool haveRemoveButton = true;
	bool haveYesButton = false;
	bool haveNoButton = false;
	bool haveTeacherButton = false;

	private IEnumerator _ShowOptions()
	{
		BaseItemScript selectedItem = SceneManager.instance.selectedItem;
		bool isBuilding = selectedItem.UI.progressUIInstance != null;

		haveInfoButton = true;
		haveRemoveButton = true;

		if (selectedItem.state == Common.State.PREVIEW)
		{
			haveYesButton = true;
			haveNoButton = true;

			if (goldPriceText != null && selectedItem.itemData != null)
			{
				goldPriceText.text = selectedItem.itemData.configuration.price.ToString();
			}

			// Tutorial handling: hide NoButton for the first building
			if (SceneManager.instance != null && SceneManager.instance.isTutorialActive && SceneManager.instance.GetBuildingCount() == 1)
			{
				haveNoButton = false;
			}

			haveInfoButton = false;
			haveUpgradeButton = false;
			haveTrainButton = false;
			haveBoostButton = false;
			haveRemoveButton = false;
			haveTeacherButton = false;
		}
		else if (isBuilding)
		{
			haveBoostButton = true;
			haveUpgradeButton = false;
			haveTrainButton = false;
			haveYesButton = false;
			haveNoButton = false;
			haveTeacherButton = false;
		}
		else
		{
			haveBoostButton = false;
			haveUpgradeButton = true;
			haveYesButton = false;
			haveNoButton = false;
			haveTeacherButton = true;

			if (selectedItem != null && selectedItem.itemData != null && ShopWindowScript.GetCategoryStringFromItemId(selectedItem.itemData.id) == "Trang trí")
			{
				haveTeacherButton = false;
			}

			if (selectedItem != null && selectedItem.assignedTeacher != null)
			{
				haveTeacherButton = false;
			}
			
			// Check if already at max level - disable upgrade button
			int nextLevel = selectedItem.level + 1;
			int maxLevel = selectedItem.itemData.configuration.levelMax;
			if (nextLevel > maxLevel)
			{
				haveUpgradeButton = false;
			}
			
			// if (selectedItem.itemData.name == "Barrack")
			// 	haveTrainButton = true;
			// else
			// 	haveTrainButton = false;
		}

		InfoButton.SetActive(haveInfoButton);
		UpgradeButton.SetActive(haveUpgradeButton);
		TrainButton.SetActive(haveTrainButton);
		BoostButton.SetActive(haveBoostButton);
		RemoveButton.SetActive(haveRemoveButton);
		YesButton.SetActive(haveYesButton);
		NoButton.SetActive(haveNoButton);
		if (TeacherButton != null) TeacherButton.SetActive(haveTeacherButton);

		if (haveInfoButton)
		{
			InfoButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveUpgradeButton)
		{
			UpgradeButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveTrainButton)
		{
			TrainButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveBoostButton)
		{
			BoostButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}
		
		if (haveTeacherButton && TeacherButton != null)
		{
			TeacherButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveRemoveButton)
		{
			RemoveButton.GetComponent<Animator>().SetTrigger("show");
		}

		if (haveYesButton)
		{
			YesButton.GetComponent<Animator>().SetTrigger("show");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveNoButton)
		{
			NoButton.GetComponent<Animator>().SetTrigger("show");
		}
	}

	public void HideOptions()
	{
		this.StartCoroutine(this._HideOptions());
	}

	private IEnumerator _HideOptions()
	{

		if (haveInfoButton)
		{
			InfoButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveUpgradeButton)
		{
			UpgradeButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveTrainButton)
		{
			TrainButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveBoostButton)
		{
			BoostButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}
		
		if (haveTeacherButton && TeacherButton != null)
		{
			TeacherButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveRemoveButton)
		{
			RemoveButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveYesButton)
		{
			YesButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		if (haveNoButton)
		{
			NoButton.GetComponent<Animator>().SetTrigger("hide");
			yield return new WaitForSeconds(_waitTime);
		}

		base.Close();
	}

	public void OnClickInfoButton()
	{
		InfoWindowScript infoWindow = UIManager.instance.ShowInfoWindow();
		if (SceneManager.instance.selectedItem != null)
		{
			infoWindow.Init(SceneManager.instance.selectedItem.itemData, SceneManager.instance.selectedItem);
		}
	}

	public void OnClickUpgradeButton()
	{
		UIManager.instance.ShowUpgradeWindow();
	}

	public void OnClickTrainButton()
	{
		UIManager.instance.ShowTrainTroopsWindow();
	}

	public void OnClickBoostButton()
	{
		UIManager.instance.ShowBoostWindow();
	}

	public void OnClickTeacherButton()
	{
		UIManager.instance.ShowCollectionWindowForAssign(SceneManager.instance.selectedItem);
	}

	public void OnClickRemoveButton()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlaySFX(SoundData.SFX_Remove_Building);
		}
		if (popupConfirm != null)
		{
			popupConfirm.SetActive(true);
		}
		if (confirmAnimator != null)
		{
			confirmAnimator.SetTrigger("show");
		}
	}

	public void OnClickConfirmRemove()
	{
		UIManager.instance.HideItemOptions();
		if (SceneManager.instance.selectedItem != null)
		{
			int buyPrice = SceneManager.instance.selectedItem.itemData.configuration.price;
			int refundAmount = buyPrice / 2;
			SceneManager.instance.AddGold(refundAmount);

			PlayerPrefs.DeleteKey("BuildingTeacher_" + SceneManager.instance.selectedItem.instanceId);
			PlayerPrefs.Save();
		}
		DataBaseManager.instance.RemoveItem(SceneManager.instance.selectedItem);
		SceneManager.instance.RemoveItem(SceneManager.instance.selectedItem);
	}

	public void OnClickCancelRemove()
	{
		if (confirmAnimator != null)
		{
			confirmAnimator.SetTrigger("hide");
		}
		if (popupConfirm != null)
		{
			popupConfirm.SetActive(false);
		}
	}

	public void OnClickYesButton()
	{
		BaseItemScript selectedItem = SceneManager.instance.selectedItem;
		if (selectedItem != null && selectedItem.state == Common.State.PREVIEW)
		{
			// Try to consume resources now
			bool canBuild = SceneManager.instance.ConsumeResource(selectedItem.itemData.configuration.resourceType, selectedItem.itemData.configuration.price);
			
			if (canBuild)
			{
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlaySFX(SoundData.SFX_Yes_Button);
				}
				selectedItem.SetState(Common.State.IDLE);
				// Show progress UI and start construction
				selectedItem.StartConstruction(null);

				// Tutorial handling: close window after confirming the first building
				if (SceneManager.instance != null && SceneManager.instance.isTutorialActive && SceneManager.instance.GetBuildingCount() == 1)
				{
					UIManager.instance.HideItemOptions();
				}
				else
				{
					// Refresh options to show standard buttons (like Boost)
					ShowOptions();
				}
			}
			else
			{
				string resourceType = selectedItem.itemData.configuration.resourceType;
				int price = selectedItem.itemData.configuration.price;
				int currentAmount = resourceType == "diamond" ? SceneManager.instance.numberOfDiamondsInStorage : SceneManager.instance.numberOfGoldInStorage;
				int missingAmount = price - currentAmount;
				
				WarningWindow warningWindow = UIManager.instance.ShowWarningWindow();
				if (warningWindow != null)
				{
					if (resourceType == "diamond")
					{
						warningWindow.SetupDiamondWarning(missingAmount, currentAmount);
					}
					else
					{
						warningWindow.SetupGoldWarning(missingAmount, currentAmount);
					}
				}
				
				// If somehow they don't have enough resources now, cancel the build
				OnClickNoButton();
			}
		}
	}

	public void OnClickNoButton()
	{
		BaseItemScript selectedItem = SceneManager.instance.selectedItem;
		if (selectedItem != null && selectedItem.state == Common.State.PREVIEW)
		{
			// Resources were never deducted, so no need to refund
			
			// Remove item
			UIManager.instance.HideItemOptions();
			SceneManager.instance.RemoveItem(selectedItem);
			Destroy(selectedItem.gameObject);
		}
	}

	public override void Close()
	{
		HideOptions();
	}
}
