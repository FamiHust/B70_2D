using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSemesterWindowScript : WindowScript
{
    public static NewSemesterWindowScript instance;

    public GameObject SubNewCategoryItemPrefab;
    public Transform ItemList;
    public Text NextSemesterLabel;


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        if (SceneManager.instance != null)
        {
            this.NextSemesterLabel.text = SceneManager.instance.currentSemester.ToString();
            this.RefreshNewItemsList();
        }
    }

    private void RefreshNewItemsList()
    {
        if (ItemList == null || SubNewCategoryItemPrefab == null) return;

        // Clear existing items
        foreach (Transform child in ItemList)
        {
            Destroy(child.gameObject);
        }

        // Get items unlocked in the current semester
        int currentSemester = SceneManager.instance.currentSemester;
        List<ItemsCollection.ItemData> newItems = Items.GetItemsBySemester(currentSemester);

        foreach (var item in newItems)
        {
            GameObject inst = Instantiate(SubNewCategoryItemPrefab, ItemList);
            SubNewCategoryItemScript script = inst.GetComponent<SubNewCategoryItemScript>();
            if (script != null)
            {
                script.SetItem(item.id);
            }
        }
    }


    public void OnClickClose()
    {
        this.Close();
    }
}

