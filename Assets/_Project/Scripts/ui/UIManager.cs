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

	/* object references */
	public GameObject WindowsContainer;

	/* private variables */
	private List<WindowScript> _windowInstances;

	void Awake()
	{
		instance = this;
		this.Design.SetActive(false);
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
		WindowScript window = Utilities.CreateInstance(prefab, this.WindowsContainer, true).GetComponent<WindowScript>();
		window.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		this._windowInstances.Add(window);
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
		SoundManager.instance.PlaySound(SoundManager.instance.Tap2, false);
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

	public void ShowInfoWindow()
	{
		this.ShowWindow(this.InfoWindow);
	}

	public void ShowUpgradeWindow()
	{
		this.ShowWindow(this.UpgradeWindow);
	}

	public void ShowBoostWindow()
	{
		this.ShowWindow(this.BoostWindow);
	}

	public ItemWindowScript ShowMapShopWindow(string areaName, List<int> itemIds, MapShopAreaScript mapShopArea = null)
	{
		ItemWindowScript window = this.ShowWindow(this.ItemWindow) as ItemWindowScript;
		window.RenderItems(areaName, itemIds, mapShopArea);
		return window;
	}
}
