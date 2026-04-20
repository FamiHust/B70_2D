/* **************************************************************************
 * ITEMS
 * **************************************************************************
 * Written by: Coppra Games
 * Created: June 2017
 * *************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour {

	public static Dictionary<int, ItemsCollection.ItemData> items;


	public static void LoadItems(){
		items = new Dictionary<int, ItemsCollection.ItemData> ();

		ItemsCollection itemsCollection = Resources.Load("ItemsCollection", typeof(ItemsCollection)) as ItemsCollection;
		if (itemsCollection != null) {
			for (int index = 0; index < itemsCollection.list.Count; index++) {
				ItemsCollection.ItemData itemData = itemsCollection.list [index];
				items.Add (itemData.id, itemData);
			}
		} else {
			Debug.LogError ("ItemsCollection is missing! please go to 'Windows/Item Editor'");
		}
	}
		
	public static List<ItemsCollection.ItemData> GetItemsBySemester(int semester)
	{
		List<ItemsCollection.ItemData> result = new List<ItemsCollection.ItemData>();
		if (items == null) return result;

		foreach (var item in items.Values)
		{
			if (item.configuration.unlockItemAtSemester == semester)
			{
				result.Add(item);
			}
		}
		return result;
	}

	public static ItemsCollection.ItemData GetItem(int itemId){

		ItemsCollection.ItemData item = null;
		items.TryGetValue (itemId, out item);
		return item;
	}

}
