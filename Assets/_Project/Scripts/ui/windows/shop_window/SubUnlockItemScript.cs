using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubUnlockItemScript : MonoBehaviour
{
	/* references */
	public Text Name;
	public RawImage Image;
	public RawImage ImageShadow;
	public Text CategoryText;

	private int _itemId;

	public void SetItem(int itemId)
	{
		this._itemId = itemId;
		ItemsCollection.ItemData itemData = Items.GetItem(itemId);
		
		if (itemData != null)
		{
			if (this.Name != null)
				this.Name.text = itemData.name;
				
			if (this.CategoryText != null)
				this.CategoryText.text = ShopWindowScript.GetCategoryStringFromItemId(itemId);
				
			if (this.Image != null)
			{
				this.Image.texture = itemData.thumb;
			}
			if (this.ImageShadow != null)
			{
				this.ImageShadow.texture = itemData.thumb;
			}
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

