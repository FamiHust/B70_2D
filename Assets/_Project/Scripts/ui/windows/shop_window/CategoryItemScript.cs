using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
	public GameObject SelectImage;


	/* private variables */
	private ShopWindowScript.Category _category;
	private Vector3 _originalImagePos;
	private bool _isInitialized = false;

	private void Initialize() {
		if (!_isInitialized) {
			if (Image != null) {
				_originalImagePos = Image.transform.localPosition;
			}
			_isInitialized = true;
		}
	}

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
			this.Name.text = "Dịch vụ";
			this.Image.sprite = this.ServiceSprite;
			break;

		case ShopWindowScript.Category.STUDENT:
			this.Name.text = "Sinh viên";
			this.Image.sprite = this.StudentSprite;
			break;
		case ShopWindowScript.Category.RESOURCES:
			this.Name.text = "Thương mại";
			this.Image.sprite = this.ResourcesSprite;
			break;
		case ShopWindowScript.Category.DECORATIONS:
			this.Name.text = "Trang trí";
			this.Image.sprite = this.DecorationsSprite;
			break;
		}
	}

	public ShopWindowScript.Category GetCategory() {
		return this._category;
	}

	public void SetActiveState(bool isActive) {
		Initialize();

		if (SelectImage != null) {
			SelectImage.SetActive(isActive);
			if (isActive) {
				SelectImage.transform.DOKill(true);
				SelectImage.transform.localScale = Vector3.one;
				SelectImage.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.3f, 10, 1f);
			}
		}
		if (Name != null) {
			Name.gameObject.SetActive(isActive);
		}
		if (Image != null) {
			Image.transform.DOKill(true);
			if (isActive) {
				Image.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f).SetEase(Ease.OutBack);
				Image.transform.DOLocalMove(_originalImagePos + new Vector3(0, 40f, 0), 0.3f).SetEase(Ease.OutBack);
			} else {
				Image.transform.localScale = Vector3.one;
				Image.transform.localPosition = _originalImagePos;
			}
		}
	}

	public void OnClick(){
		this.GetComponentInParent<ShopWindowScript> ().OnClickCategory (this._category);
	}

}
