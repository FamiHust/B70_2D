using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemCollectNotificationUIScript : MonoBehaviour
{
   
	/* object references */
	public Transform Container;
	public GameObject EventIcon;
	public GameObject GoldIcon;
	public GameObject ElixirIcon;
	public GameObject HappyIcon;
	public GameObject EduIcon;
	public Collider NotificationCollider;

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
		bool isEvent = (type == "event");

		// EventIcon ưu tiên cao nhất — khi là event, ẩn tất cả icon khác
		if (this.EventIcon != null) this.EventIcon.SetActive(isEvent);

		this.GoldIcon.SetActive(!isEvent && type == "gold");
		this.ElixirIcon.SetActive(!isEvent && type == "elixir");
		if (this.HappyIcon != null) this.HappyIcon.SetActive(!isEvent && type == "happy");
		if (this.EduIcon != null) this.EduIcon.SetActive(!isEvent && (type == "education" || type == "academic" || type == "edu"));
	}
    
}
