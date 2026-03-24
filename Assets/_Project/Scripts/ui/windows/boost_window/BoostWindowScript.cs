using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostWindowScript : WindowScript
{
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
}
