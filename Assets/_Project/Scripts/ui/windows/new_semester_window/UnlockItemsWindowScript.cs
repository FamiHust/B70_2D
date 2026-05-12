using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockItemsWindowScript : WindowScript
{
    public static UnlockItemsWindowScript instance;

    public GameObject SubUnlockItemPrefab;
    public Transform ItemList;
    public Text NextSemesterLabel;
    public Animator anim;


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        if (SceneManager.instance != null)
        {
            this.NextSemesterLabel.text = SceneManager.instance.currentLevel.ToString();
            this.RefreshNewItemsList();
            this.ShowWindow();
        }
    }

    private void RefreshNewItemsList()
    {
        if (ItemList == null || SubUnlockItemPrefab == null) return;

        // Clear existing items
        foreach (Transform child in ItemList)
        {
            Destroy(child.gameObject);
        }

        // Get items unlocked in the current level
        int currentLevel = SceneManager.instance.currentLevel;
        List<ItemsCollection.ItemData> newItems = Items.GetItemsBySemester(currentLevel);

        foreach (var item in newItems)
        {
            GameObject inst = Instantiate(SubUnlockItemPrefab, ItemList);
            SubUnlockItemScript script = inst.GetComponent<SubUnlockItemScript>();
            if (script != null)
            {
                script.SetItem(item.id);
            }
        }
    }


    public void OnClickClose()
    {
        this.HideWindow();
        this.Close();

        // Nếu vừa lên Level 2, sau khi đóng bảng Unlock thì mới hiện Tutorial tiếp theo
        if (SceneManager.instance != null && SceneManager.instance.currentLevel == 2)
        {
            if (GameOverlayWindowScript.instance != null)
            {
                GameOverlayWindowScript.instance.TriggerTutorialAfterUnlock();
            }
        }
    }

    public void HideWindow()
    {
        if (anim != null) anim.Play("Hide");
    }

    public void ShowWindow()
    {
        if (anim != null) anim.Play("Show");
    }
}
