using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubNewCategoryItemScript : MonoBehaviour
{
	/* sprites (copy from SubCategoryItemScript for consistency) */
	public Sprite D4Sprite;
	public Sprite C2Sprite;
	public Sprite C3Sprite;
	public Sprite C3BSprite;
	public Sprite C5Sprite;
	public Sprite C6Sprite;
	public Sprite C9Sprite;
	public Sprite C10Sprite;
	public Sprite D35Sprite;
	public Sprite D6Sprite;
	public Sprite D8Sprite;
	public Sprite C7Sprite;
	public Sprite B8Sprite;
	public Sprite C4Sprite;
	public Sprite CanteenSprite;
	public Sprite GaraD6Sprite;
	public Sprite C1Sprite;
	public Sprite GiaiPhongGateSprite;
	public Sprite TDNGateSprite;
	public Sprite LibSprite;
	public Sprite WallSprite;
	public Sprite Tree3Sprite;
	public Sprite ITIMSSprite;
	public Sprite SecurityRoomSprite;
	public Sprite PCLabSprite;

	/* references */
	public Text Name;
	public Image Image;

	private int _itemId;

	public void SetItem(int itemId)
	{
		this._itemId = itemId;
		ItemsCollection.ItemData itemData = Items.GetItem(itemId);
		
		if (itemData != null)
		{
			if (this.Name != null)
				this.Name.text = itemData.name;
				
			this.SetImageSprite(itemId);
		}
	}

	private void SetImageSprite(int itemId)
	{
		if (this.Image == null) return;

		switch (itemId)
		{
			case 3635: this.Image.sprite = this.D4Sprite; break;
			case 8216: this.Image.sprite = this.C2Sprite; break;
			case 2454: this.Image.sprite = this.C3Sprite; break;
			case 5835: this.Image.sprite = this.C3BSprite; break;
			case 3265: this.Image.sprite = this.C4Sprite; break;
			case 3504: this.Image.sprite = this.C5Sprite; break;
			case 2617: this.Image.sprite = this.C6Sprite; break;
			case 3336: this.Image.sprite = this.C7Sprite; break;
			case 9295: this.Image.sprite = this.C9Sprite; break;
			case 8385: this.Image.sprite = this.C10Sprite; break;
			case 4407: this.Image.sprite = this.D35Sprite; break;
			case 6330: this.Image.sprite = this.D6Sprite; break;
			case 5134: this.Image.sprite = this.D8Sprite; break;
			case 5342: this.Image.sprite = this.B8Sprite; break;
			case 1399: this.Image.sprite = this.CanteenSprite; break;
			case 4132: this.Image.sprite = this.GaraD6Sprite; break;
			case 2496: this.Image.sprite = this.C1Sprite; break;
			case 2949: this.Image.sprite = this.GiaiPhongGateSprite; break;
			case 1251: this.Image.sprite = this.TDNGateSprite; break;
			case 6677: this.Image.sprite = this.LibSprite; break;
			case 7666: this.Image.sprite = this.WallSprite; break;
			case 5341: this.Image.sprite = this.Tree3Sprite; break;
			case 3090: this.Image.sprite = this.ITIMSSprite; break;
			case 1628: this.Image.sprite = this.SecurityRoomSprite; break;
			case 9138: this.Image.sprite = this.PCLabSprite; break;
		}
	}
	public void OnClickInfoButton()
	{
		ItemsCollection.ItemData itemData = Items.GetItem(this._itemId);
		if (itemData != null)
		{
			InfoWindowScript infoWindow = UIManager.instance.ShowInfoWindow();
			infoWindow.Init(itemData);
		}
	}
}

