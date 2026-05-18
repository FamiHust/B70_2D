using System.Collections.Generic;
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

        [Header("Options")]
        [Tooltip("Container chứa các option item được sinh ra động")]
        public Transform optionContainer;
        [Tooltip("Prefab của mỗi option item (cần có OptionItemUI component)")]
        public GameObject optionItemPrefab;

        private readonly List<OptionItemUI> _spawnedOptionItems = new List<OptionItemUI>();

        /// <summary>
        /// Thiết lập dữ liệu cho Event instance này.
        /// </summary>
        public void Setup(UniversityEventData data)
        {
            this.eventData = data;

            if (eventNameText != null) eventNameText.text = data.eventName;
            if (descriptionText != null) descriptionText.text = data.description;

            // Xóa option items cũ
            foreach (var old in _spawnedOptionItems)
                if (old != null) Destroy(old.gameObject);
            _spawnedOptionItems.Clear();

            // Sinh option items mới
            if (data.options == null || optionContainer == null || optionItemPrefab == null) return;

            for (int i = 0; i < data.options.Count; i++)
            {
                GameObject item = Instantiate(optionItemPrefab, optionContainer);

                OptionItemUI optUI = item.GetComponent<OptionItemUI>();
                if (optUI != null)
                {
                    _spawnedOptionItems.Add(optUI);
                    int capturedIndex = i;
                    optUI.Setup(data.options[i], i + 1, () => OnClickOption(capturedIndex));
                }
            }

            // Wire hint: khi 1 item show hint thì ForceHide tất cả item còn lại
            foreach (var item in _spawnedOptionItems)
            {
                OptionItemUI captured = item;
                item.onBeforeShow = () =>
                {
                    foreach (var other in _spawnedOptionItems)
                        if (other != captured) other.ForceHide();
                };
            }
        }

        public void OnClickClose()
        {
            this.Close();
        }

        /// <summary>
        /// Gắn vào Button của từng Option. Truyền index tương ứng (0, 1, 2...).
        /// </summary>
        public void OnClickOption(int optionIndex)
        {
            if (eventData != null && UniversityEventManager.instance != null)
            {
                if (eventData.options != null && optionIndex >= 0 && optionIndex < eventData.options.Count)
                {
                    UniversityEventManager.instance.ApplyOptionEffects(eventData.options[optionIndex]);

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
