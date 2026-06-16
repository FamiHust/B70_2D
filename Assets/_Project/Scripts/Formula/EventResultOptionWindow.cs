using System;
using UnityEngine;
using UnityEngine.UI;

namespace B70.Balance
{
    public class EventResultOptionWindow : WindowScript
    {
        [Header("UI References")]
        public Text eventNameText;
        public Text optionText;
        public Text descriptionText;

        public Text goldText;
        public Text happinessText;
        public Text educationText;

        [Header("Button")]
        public Button selectButton;

        public Animator anim;

        private EventOption _currentOption;
        private BaseItemScript _sourceBuilding; // Tòa nhà đã kích hoạt event

        private Color _defaultGoldColor = Color.white;
        private Color _defaultHappinessColor = Color.white;
        private Color _defaultEducationColor = Color.white;
        private bool _colorsInitialized = false;

        private void SetTextColor(Text textComponent, float modifierValue, Color defaultColor)
        {
            if (textComponent == null) return;
            if (modifierValue > 0) textComponent.color = Color.green;
            else if (modifierValue < 0) textComponent.color = Color.red;
            else textComponent.color = defaultColor;
        }

        public void Setup(string eventName, EventOption option, int optionIndex, BaseItemScript sourceBuilding = null)
        {
            _sourceBuilding = sourceBuilding;
            if (!_colorsInitialized)
            {
                if (goldText != null) _defaultGoldColor = goldText.color;
                if (happinessText != null) _defaultHappinessColor = happinessText.color;
                if (educationText != null) _defaultEducationColor = educationText.color;
                _colorsInitialized = true;
            }

            _currentOption = option;

            if (eventNameText != null)
                eventNameText.text = eventName;

            if (optionText != null)
                optionText.text = "Lựa chọn " + optionIndex + ": " + option.title;

            if (descriptionText != null)
                descriptionText.text = option.description;

            if (goldText != null)
            {
                goldText.gameObject.SetActive(true);
                goldText.text = FormatValue(option.goldModifier);
                SetTextColor(goldText, option.goldModifier, _defaultGoldColor);
            }

            int happyMod = Mathf.RoundToInt(option.happinessModifier);
            if (happinessText != null)
            {
                happinessText.gameObject.SetActive(true);
                happinessText.text = FormatValue(happyMod) + "%";
                SetTextColor(happinessText, happyMod, _defaultHappinessColor);
            }

            int eduMod = Mathf.RoundToInt(option.educationModifier);
            if (educationText != null)
            {
                educationText.gameObject.SetActive(true);
                educationText.text = FormatValue(eduMod) + "%";
                SetTextColor(educationText, eduMod, _defaultEducationColor);
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnSelectClicked);
            }
        }

        private void OnSelectClicked()
        {
            if (_currentOption != null && UniversityEventManager.instance != null)
            {
                UniversityEventManager.instance.ApplyOptionEffects(_currentOption);

                if (SceneManager.instance != null)
                {
                    SceneManager.instance.SaveResources();
                    SceneManager.instance.RefreshResourceUIs("gold");
                    SceneManager.instance.RefreshResourceUIs("happy");
                    SceneManager.instance.RefreshResourceUIs("education");
                }
            }

            // Thông báo tòa nhà rằng event đã được giải quyết
            // → tắt EventIcon, hiển lại icon thu thập bình thường nếu có
            if (_sourceBuilding != null && _sourceBuilding.Production != null)
            {
                _sourceBuilding.Production.ResolveEvent();
            }

            this.Close();
        }

        private static string FormatValue(int value)
        {
            if (value > 0) return $"+{value}";
            return value.ToString(); 
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
