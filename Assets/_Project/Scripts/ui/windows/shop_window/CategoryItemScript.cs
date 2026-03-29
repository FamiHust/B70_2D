using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryItemScript : MonoBehaviour {
	/* prefabs */
	// public Sprite ArmySprite;
	// public Sprite DefenceSprite;
	public Sprite ServiceSprite;
	public Sprite ResourcesSprite;
	public Sprite StudentSprite;
	public Sprite DecorationsSprite;

	/* references */
	public Text Name;
	public Image Image;


	/* private variables */
	private ShopWindowScript.Category _category;

	public void SetCategory(ShopWindowScript.Category category){
		this._category = category;

		switch (this._category) {
		// case ShopWindowScript.Category.ARMY:
		// 	this.Name.text = "ARMY";
		// 	this.Image.sprite = this.ArmySprite;
		// 	break;
		// case ShopWindowScript.Category.DEFENCE:
		// 	this.Name.text = "DEFENCE";
		// 	this.Image.sprite = this.DefenceSprite;
		// 	break;
		case ShopWindowScript.Category.SERVICE:
			this.Name.text = "SERVICE";
			this.Image.sprite = this.ServiceSprite;
			break;
		case ShopWindowScript.Category.RESOURCES:
			this.Name.text = "RESOURCES";
			this.Image.sprite = this.ResourcesSprite;
			break;
		case ShopWindowScript.Category.STUDENT:
			this.Name.text = "STUDENT";
			this.Image.sprite = this.StudentSprite;
			break;
		case ShopWindowScript.Category.DECORATIONS:
			this.Name.text = "DECORATIONS";
			this.Image.sprite = this.DecorationsSprite;
			break;
		}
	}

	public void OnClick(){
		this.GetComponentInParent<ShopWindowScript> ().OnClickCategory (this._category);
	}

}
