using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

	public static UIManager instance;

	/* prefabs */
	public GameObject Design;

	public GameObject GameOverlayWindow;
	public GameObject AttackOverlayWindow;
	public GameObject ShopWindow;
	public GameObject ItemWindow;
	public GameObject SceneEnteringWindow;
	public GameObject BuildersBusyWindow;
	public GameObject ResultWindow;
	public GameObject TrainTroopsWindow;
	public GameObject ItemOptionsWindow;
	public GameObject InfoWindow;
	public GameObject UpgradeWindow;
	public GameObject BoostWindow;
	public GameObject TutorialWindow;
	public GameObject UnlockItemWindow;
	public GameObject NewSemesterWindow;
	public GameObject MissionWindow;
	public GameObject EventWindow;
	public GameObject EventResultOptionWindow;


	/* object references */
	public GameObject WindowsContainer;

	/* private variables */
	private List<WindowScript> _windowInstances;

	void Awake()
	{
		instance = this;
		this._windowInstances = new List<WindowScript>();

		// show menu window at start if assigned
		if (this.SceneEnteringWindow == null)
		{
			// no scene entering window assigned - still allow showing menu
		}
		// MenuWindow prefab is in SceneManager; UIManager will not create it here directly
	}

	/// <summary>
	/// Instantiate the window instance.
	/// </summary>
	/// <returns>The window.</returns>
	/// <param name="prefab">Prefab.</param>
	public WindowScript ShowWindow(GameObject prefab)
	{
		// Automatically close other windows except the overlay before showing the new one
		// but don't close windows if we are opening the overlay itself or an Info popup
		if (prefab != this.GameOverlayWindow && prefab != this.InfoWindow && prefab != this.UpgradeWindow && prefab != this.BoostWindow && prefab != this.TutorialWindow && prefab != this.EventWindow)
		{
			this.CloseAllWindowsExceptOverlay();
		}

		WindowScript window = Utilities.CreateInstance(prefab, this.WindowsContainer, true).GetComponent<WindowScript>();
		window.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		this._windowInstances.Add(window);

		// Hide GameOverlay when another window opens
		if (prefab != this.GameOverlayWindow && prefab != this.EventWindow && GameOverlayWindowScript.instance != null)
		{
			GameOverlayWindowScript.instance.HideOverlay();
		}

		if (prefab != this.GameOverlayWindow && TimeManager.instance != null)
		{
			TimeManager.instance.SetPaused(true);
		}

		return window;
	}

	/// <summary>
	/// Shows the game overlay window.
	/// </summary>
	public void ShowGameOverlayWindow()
	{
		this.ShowWindow(this.GameOverlayWindow);
	}

	public void ShowAttackOverlayWindow()
	{
		this.ShowWindow(this.AttackOverlayWindow);
	}


	/// <summary>
	/// Shows the shop widow.
	/// </summary>
	public void ShowShopWidow()
	{
		this.ShowWindow(this.ShopWindow);

	}

	public void CloseAllWindows()
	{
		foreach (WindowScript window in this._windowInstances)
		{
			if (window != null)
			{
				window.Close();
			}
		}
		this._windowInstances = new List<WindowScript>();
	}

	public void CloseAllWindowsExceptOverlay()
	{
		List<WindowScript> remainingWindows = new List<WindowScript>();
		foreach (WindowScript window in this._windowInstances)
		{
			if (window != null)
			{
				// Keep the GameOverlayWindow and TutorialWindow instances, close everything else
				if (window is GameOverlayWindowScript || window is TutorialWindowScript)
				{
					remainingWindows.Add(window);
				}
				else
				{
					window.Close();
				}
			}
		}
		this._windowInstances = remainingWindows;
	}

	public void ShowSceneEnteringWindow(Action intermediateCallback)
	{
		SceneEnteringWindowScript window = this.ShowWindow(this.SceneEnteringWindow) as SceneEnteringWindowScript;
		window.OnIntermediate += intermediateCallback;
	}

	public void ShowBuildersBusyWindow()
	{
		this.ShowWindow(this.BuildersBusyWindow);
	}

	public void ShowResultWindow(bool victory, int swordManExpended, int archerExpended)
	{
		this.ShowWindow(this.ResultWindow);
		ResultWindowScript.instance.SetData(victory, swordManExpended, archerExpended);
	}

	public void ShowTrainTroopsWindow()
	{
		this.ShowWindow(this.TrainTroopsWindow);
	}

	public void ShowItemOptions()
	{
		this.ShowWindow(this.ItemOptionsWindow);
	}

	public void HideItemOptions()
	{
		if (ItemOptionsWindowScript.instance != null)
		{
			ItemOptionsWindowScript.instance.Close();
		}
	}

	public InfoWindowScript ShowInfoWindow()
	{
		return this.ShowWindow(this.InfoWindow) as InfoWindowScript;
	}


	public void ShowUpgradeWindow()
	{
		this.ShowWindow(this.UpgradeWindow);
	}

	public void ShowBoostWindow()
	{
		this.ShowWindow(this.BoostWindow);
	}

	public WindowScript ShowTutorialWindow()
	{
		return this.ShowWindow(this.TutorialWindow);
	}

	public void ShowNewSemesterWindow(B70.Balance.SemesterBreakdown bd, int semesterNumber, float happiness, float education)
	{
		NewSemesterWindowScript window = this.ShowWindow(this.NewSemesterWindow) as NewSemesterWindowScript;
		if (window != null)
		{
			window.Setup(bd, semesterNumber, happiness, education);
		}
	}

	public void ShowUnlockItemsWindow()
	{
		this.ShowWindow(this.UnlockItemWindow);
	}


	public WindowScript ShowMissionWindow()
	{
		return this.ShowWindow(this.MissionWindow);
	}

	public void ShowEventWindow()
	{
		this.ShowWindow(this.EventWindow);
	}

	public B70.Balance.EventResultOptionWindow ShowEventResultOptionWindow()
	{
		return this.ShowWindow(this.EventResultOptionWindow) as B70.Balance.EventResultOptionWindow;
	}

	public ItemWindowScript ShowMapShopWindow(string areaName, List<int> itemIds, MapShopAreaScript mapShopArea = null)
	{
		ItemWindowScript window = this.ShowWindow(this.ItemWindow) as ItemWindowScript;
		window.RenderItems(areaName, itemIds, mapShopArea);
		return window;
	}

	public IEnumerator CheckWindowsAfterClose()
	{
		yield return new WaitForEndOfFrame();
		
		if (_windowInstances != null)
		{
			_windowInstances.RemoveAll(w => w == null);
			
			bool hasOtherWindow = false;
			foreach (var w in _windowInstances)
			{
				if (w != null && !(w is GameOverlayWindowScript))
				{
					hasOtherWindow = true;
					break;
				}
			}

			if (!hasOtherWindow && GameOverlayWindowScript.instance != null)
			{
				// During tutorial, if building is under construction, don't show normal overlay
				if (SceneManager.instance != null && SceneManager.instance.isTutorialActive && SceneManager.instance.IsAnyBuildingUnderConstruction())
				{
					// Stay hidden
				}
				else
				{
					GameOverlayWindowScript.instance.ShowOverlay();
				}
			}

			if (TimeManager.instance != null)
			{
				TimeManager.instance.SetPaused(hasOtherWindow);
			}
		}
	}
}
