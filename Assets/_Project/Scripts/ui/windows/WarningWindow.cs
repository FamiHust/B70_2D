using UnityEngine;
using UnityEngine.UI;

public class WarningWindow : WindowScript
{
    [Header("UI Texts")]
    public Text TitleText;
    public Text RequireText;
    public Text GoldText;
    public Text DiamondText;

    [Header("Panels")]
    public GameObject GoldPanel;
    public GameObject DiamondPanel;

    public void SetupGoldWarning(int missingAmount, int currentGold)
    {
        if (TitleText != null) TitleText.text = "Không đủ vàng";
        if (RequireText != null) RequireText.text = "Thiếu : " + missingAmount + " vàng";
        if (GoldText != null) GoldText.text = currentGold.ToString();
        
        if (GoldPanel != null) GoldPanel.SetActive(true);
        if (DiamondPanel != null) DiamondPanel.SetActive(false);
    }

    public void SetupDiamondWarning(int missingAmount, int currentDiamond)
    {
        if (TitleText != null) TitleText.text = "Không đủ tiền";
        if (RequireText != null) RequireText.text = "Thiếu : " + missingAmount + " tiền";
        if (DiamondText != null) DiamondText.text = currentDiamond.ToString();
        
        if (GoldPanel != null) GoldPanel.SetActive(false);
        if (DiamondPanel != null) DiamondPanel.SetActive(true);
    }
}
