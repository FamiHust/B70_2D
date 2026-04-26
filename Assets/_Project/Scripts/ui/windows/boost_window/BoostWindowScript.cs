using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostWindowScript : WindowScript
{
    public ProgressPanelScript GoldInfo;
    public ProgressPanelScript DiamondInfo;
    public ProgressPanelScript StudentInfo;
    public Animator anim;

    void Start()
    {
        if (this.GoldInfo != null && SceneManager.instance != null)
        {
            this.GoldInfo.hasMaxValue = true;
            this.GoldInfo.maxValue = SceneManager.instance.goldStorageCapacity;
            this.GoldInfo.value = SceneManager.instance.numberOfGoldInStorage;
        }

        if (this.DiamondInfo != null && SceneManager.instance != null)
        {
            this.DiamondInfo.hasMaxValue = true;
            this.DiamondInfo.maxValue = SceneManager.instance.diamondStorageCapacity;
            this.DiamondInfo.value = SceneManager.instance.numberOfDiamondsInStorage;
        }

        if (this.StudentInfo != null && SceneManager.instance != null)
        {
            this.StudentInfo.hasMaxValue = true;
            this.StudentInfo.maxValue = SceneManager.instance.studentStorageCapacity;
            this.StudentInfo.value = SceneManager.instance.numberOfStudentInStorage;
            this.StudentInfo.showAsCurrentMax = true;
        }
    }

    public void OnClickBoostButton()
    {
        BaseItemScript selectedItem = SceneManager.instance.selectedItem;
        if (selectedItem == null) return;

        // Check if building is currently under construction
        if (selectedItem.UI.progressUIInstance == null)
        {
            Debug.Log("Building is not under construction, cannot boost.");
            return;
        }

        // Consume diamonds (5 diamonds to boost)
        if (SceneManager.instance.ConsumeResource("diamond", 5))
        {
            selectedItem.FinishConstruction();

            // Close window and item options
            this.Close();
            UIManager.instance.HideItemOptions();
        }
        else
        {
            Debug.Log("Not enough diamonds to boost!");
        }
    }
    public override void Close()
    {
        base.Close();
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
