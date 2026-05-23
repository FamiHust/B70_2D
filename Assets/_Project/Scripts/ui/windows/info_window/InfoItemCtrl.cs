using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoItemCtrl : MonoBehaviour {
	/* object references */
	public Text Property;
	public Text Value;
	public Image Icon;

	public void SetData(string property, string value, Sprite icon = null)
	{
		this.Property.text = property;
		this.Value.text = value;
		
		if (this.Icon != null)
		{
			if (icon != null)
			{
				this.Icon.sprite = icon;
				this.Icon.gameObject.SetActive(true);
			}
			else
			{
				this.Icon.gameObject.SetActive(false);
			}
		}
	}
}
