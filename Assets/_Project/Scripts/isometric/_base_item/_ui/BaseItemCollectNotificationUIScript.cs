using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemCollectNotificationUIScript : MonoBehaviour
{
   
	/* object references */
	public Transform Container;
	public GameObject GoldIcon;
	public GameObject ElixirIcon;

	/* private vars */
	private BaseItemScript _baseItem;


	void Start()
	{
		this._baseItem = this.GetComponentInParent<BaseItemScript>();
		if (this._baseItem == null)
		{
			return;
		}

		float gw = this._baseItem.itemData.gridWidth;
		float gh = this._baseItem.itemData.gridHeight;
		this.transform.localPosition = new Vector3((gw - 1f) / 2f, this.transform.localPosition.y, (gh - 1f) / 2f);
	}

    public void SetIcon(string type)
	{
		this.GoldIcon.SetActive(type == "gold");
		this.ElixirIcon.SetActive(type == "elixir");
	}
    
}
