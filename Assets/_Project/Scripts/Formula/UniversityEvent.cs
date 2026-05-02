using UnityEngine;
using UnityEngine.UI;

namespace B70.Balance
{
    /// <summary>
    /// Component gắn vào Event Prefab để lưu trữ dữ liệu từ UniversityEventData.
    /// </summary>
    public class UniversityEvent : WindowScript
    {
        [Header("Data Reference")]
        public UniversityEventData eventData;

        [Header("UI References")]
        public Text eventNameText;
        public Text descriptionText;
        public Text happinessText;
        public Text educationText;
        public Text goldText;

        /// <summary>
        /// Thiết lập dữ liệu cho Event instance này.
        /// </summary>
        public void Setup(UniversityEventData data)
        {
            this.eventData = data;
            
            if (eventNameText != null) eventNameText.text = data.eventName;
            if (descriptionText != null) descriptionText.text = data.description;
            
            if (happinessText != null) 
                happinessText.text = "Happy: " + (data.happinessModifier >= 0 ? "+" : "") + data.happinessModifier.ToString("F0") + "%";
                
            if (educationText != null) 
                educationText.text = "Academic: " + (data.educationModifier >= 0 ? "+" : "") + data.educationModifier.ToString("F0") + "%";
                
            if (goldText != null) 
                goldText.text = "Gold: " + (data.goldCost > 0 ? "-" : "") + data.goldCost.ToString();
        }

        public void OnClickClose()
        {
            if (eventData != null && UniversityEventManager.instance != null)
            {
                UniversityEventManager.instance.ApplyEventEffects(eventData);

                if (SceneManager.instance != null)
                {
                    SceneManager.instance.SaveResources();
                    SceneManager.instance.RefreshResourceUIs("happy");
                    SceneManager.instance.RefreshResourceUIs("education");
                }
            }
            this.Close();
        }

        public void OnClickAccept()
        {
            if (eventData != null && UniversityEventManager.instance != null)
            {
                UniversityEventManager.instance.ApplyEventEffects(eventData);

                if (SceneManager.instance != null)
                {
                    SceneManager.instance.SaveResources();
                    SceneManager.instance.RefreshResourceUIs("gold");
                }
            }
            this.Close();
        }
    }
}
