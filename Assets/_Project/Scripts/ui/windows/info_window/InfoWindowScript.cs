using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindowScript : WindowScript
{
	public static InfoWindowScript instance;

	/* prefabs */
	public GameObject InfoItem;

	/* object references */
	public Text Title;
	public Text LevelText;
	public RawImage ThumbImage;
	public GameObject InfoPanel;
	public Animator anim;


	/* private vars */
	private BaseItemScript _baseItem;
	private ItemsCollection.ItemData _itemData;

	void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void Init(ItemsCollection.ItemData itemData, BaseItemScript baseItem = null)
	{
		this._itemData = itemData;
		this._baseItem = baseItem;
		this.RenderInfo();
	}

	public void RenderInfo()
	{
		if (this._itemData == null) return;

		// Clear previous items
		foreach (Transform child in this.InfoPanel.transform)
		{
			Destroy(child.gameObject);
		}

		this.Title.text = this._itemData.name;

		int currentLevel = this._baseItem != null ? this._baseItem.level : 1;
		if (this.LevelText != null)
		{
			this.LevelText.text = currentLevel.ToString();
		}

		this.ThumbImage.texture = this._itemData.thumb;

		bool isCharacter = this._itemData.configuration.isCharacter;

		if (!isCharacter)
		{
			//GRID SIZE
			string gridSize = this._itemData.gridWidth.ToString() + "x" + this._itemData.gridHeight.ToString();
			this._CreateInfoItem("Grid Size", gridSize);
		}

		string buildTime = this._itemData.configuration.buildTime.ToString() + "s";
		this._CreateInfoItem("Build Time", buildTime);

		if (this._itemData.configuration.speed > 0)
		{
			string speed = this._itemData.configuration.speed.ToString();
			this._CreateInfoItem("Speed", speed);
		}

		if (this._itemData.configuration.attackRange > 0)
		{
			string attackRange = this._itemData.configuration.attackRange.ToString();
			this._CreateInfoItem("Attack Range", attackRange);
		}

		if (this._itemData.configuration.defenceRange > 0)
		{
			string defenceRange = this._itemData.configuration.defenceRange.ToString();
			this._CreateInfoItem("Defence Range", defenceRange);
		}

		if (this._itemData.configuration.hitPoints > 0)
		{
			string hitPoints = this._itemData.configuration.hitPoints.ToString();
			this._CreateInfoItem("Hit Points", hitPoints);
		}

		if (this._itemData.configuration.productionRate > 0)
		{
			string productionRate = this._itemData.configuration.productionRate.ToString();
			this._CreateInfoItem("Production Rate", productionRate);

			string product = this._itemData.configuration.product;
			this._CreateInfoItem("Product", product);
		}

		if (!string.IsNullOrEmpty(this._itemData.description))
			this._CreateInfoItem("Description", this._itemData.description);

		if (this._baseItem != null)
		{
			this._CreateInfoItem("Current Level", this._baseItem.level.ToString());
		}
	}


	private void _CreateInfoItem(string property, string value)
	{
		InfoItemCtrl comp = Utilities.CreateInstance(this.InfoItem, this.InfoPanel, true).GetComponent<InfoItemCtrl>();
		comp.SetData(property, value);
	}

	public void HideWindow()
	{
		if (anim != null) anim.Play("Hide");
	}

	public void CloseWindow()
	{
		if (anim != null) anim.Play("Close");
	}

	public void ShowWindow()
	{
		if (anim != null) anim.Play("Show");
	}
}
