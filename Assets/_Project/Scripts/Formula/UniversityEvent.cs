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
        public Image eventImage;
        public Text eventNameText;
        public Text descriptionText;

        [Header("Options")]
        [Tooltip("Container chứa các option item được sinh ra động")]
        public Transform optionContainer;
        [Tooltip("Prefab của mỗi option item (cần có OptionItemUI component)")]
        public GameObject optionItemPrefab;

        public Animator anim;

        private readonly List<OptionItemUI> _spawnedOptionItems = new List<OptionItemUI>();

        /// <summary>
        /// Thiết lập dữ liệu cho Event instance này.
        /// </summary>
        public void Setup(UniversityEventData data)
        {
            this.eventData = data;

            if (eventNameText != null) eventNameText.text = data.eventName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (eventImage != null && data.eventSprite != null) eventImage.sprite = data.eventSprite;

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
            if (eventData != null && UIManager.instance != null)
            {
                if (eventData.options != null && optionIndex >= 0 && optionIndex < eventData.options.Count)
                {
                    EventOption selectedOption = eventData.options[optionIndex];
                    EventResultOptionWindow resultWindow = UIManager.instance.ShowEventResultOptionWindow();
                    if (resultWindow != null)
                    {
                        resultWindow.Setup(eventData.eventName, selectedOption, optionIndex + 1);
                    }
                }
            }
            this.Close();
        }

        public void Show()
        {
            if (anim != null) anim.Play("Show");
        }

        public void Hide()
        {
            if (anim != null) anim.Play("Hide");
        }
    }
}
