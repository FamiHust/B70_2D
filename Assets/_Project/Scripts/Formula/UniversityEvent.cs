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
        }

        public void OnClickClose()
        {
            this.Close();
        }

        public void OnClickAccept()
        {
            this.Close();
        }

        /// <summary>
        /// Hàm gắn vào các Button Option trên UI. Truyền index tương ứng của Option (0, 1, 2...).
        /// </summary>
        public void OnClickOption(int optionIndex)
        {
            if (eventData != null && UniversityEventManager.instance != null)
            {
                if (eventData.options != null && optionIndex >= 0 && optionIndex < eventData.options.Count)
                {
                    var option = eventData.options[optionIndex];
                    UniversityEventManager.instance.ApplyOptionEffects(option);

                    if (SceneManager.instance != null)
                    {
                        SceneManager.instance.SaveResources();
                        SceneManager.instance.RefreshResourceUIs("gold");
                        SceneManager.instance.RefreshResourceUIs("happy");
                        SceneManager.instance.RefreshResourceUIs("education");
                    }
                }
            }
            this.Close();
        }
    }
}
