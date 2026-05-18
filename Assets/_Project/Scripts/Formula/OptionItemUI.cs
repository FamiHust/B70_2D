using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace B70.Balance
{
    /// <summary>
    /// Component gắn lên Option Item Prefab trong UI Event.
    /// Hiển thị title, description và các modifier (gold, happiness, education) của một EventOption.
    /// Gọi Setup() từ UniversityEvent khi sinh động.
    /// </summary>
    public class OptionItemUI : MonoBehaviour
    {
        [Header("UI References")]
        public Text indexText;
        public Text titleText;
        public Text descriptionText;

        public Text goldText;
        public Text happinessText;
        public Text educationText;

        [Header("Background")]
        [Tooltip("Image nền của option item")]
        public Image backgroundOption;
        [Tooltip("Danh sách sprite theo thứ tự option (index 0 = option 1, ...)")]
        public Sprite[] optionSprites;

        [Header("Button")]
        public Button selectButton;

        [Header("Hint")]
        public GameObject hintZone;
        public Button hintButton;

        [Header("Hint Tween Settings")]
        public float tweenDuration = 0.3f;

        private bool _hintVisible;
        private Tweener _hintTween;

        /// <summary>
        /// Gán từ UniversityEvent để đóng các HintZone anh em trước khi mở hint này.
        /// </summary>
        public Action onBeforeShow;

        private void Awake()
        {
            // Đảm bảo HintZone ẩn ban đầu (scale = 0)
            if (hintZone != null)
            {
                hintZone.transform.localScale = Vector3.zero;
                hintZone.SetActive(false);
            }

            if (hintButton != null)
                hintButton.onClick.AddListener(ToggleHint);
        }

        /// <summary>
        /// Điền dữ liệu từ EventOption và đăng ký callback khi người dùng nhấn chọn.
        /// </summary>
        public void Setup(EventOption option, int index, Action onSelect)
        {
            if (indexText != null)
                indexText.text = index.ToString();

            // Background sprite theo thứ tự
            if (backgroundOption != null && optionSprites != null)
            {
                int spriteIndex = index - 1; // index là 1-based
                if (spriteIndex >= 0 && spriteIndex < optionSprites.Length)
                    backgroundOption.sprite = optionSprites[spriteIndex];
            }

            if (titleText != null)
                titleText.text = option.title;

            if (descriptionText != null)
                descriptionText.text = option.description;

            if (goldText != null)
                goldText.text      = FormatValue(option.goldModifier);
            if (happinessText != null)
                happinessText.text = FormatValue(Mathf.RoundToInt(option.happinessModifier)) + "%";
            if (educationText != null)
                educationText.text = FormatValue(Mathf.RoundToInt(option.educationModifier)) + "%";

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => onSelect?.Invoke());
            }
        }

        // ── Hint Toggle ──────────────────────────────────────────────────────

        public void ToggleHint()
        {
            if (_hintVisible)
            {
                HideHint();
            }
            else
            {
                onBeforeShow?.Invoke(); // đóng các hint khác trước
                ShowHint();
            }
        }

        /// <summary>
        /// Tắt HintZone từ bên ngoài (không toggle, chỉ hide).
        /// </summary>
        public void ForceHide()
        {
            if (_hintVisible) HideHint();
        }

        private void ShowHint()
        {
            if (hintZone == null) return;
            _hintVisible = true;
            _hintTween?.Kill();

            hintZone.SetActive(true);
            hintZone.transform.localScale = Vector3.zero;
            _hintTween = hintZone.transform
                .DOScale(Vector3.one, tweenDuration)
                .SetEase(Ease.OutBack);
        }

        private void HideHint()
        {
            if (hintZone == null) return;
            _hintVisible = false;
            hintZone.SetActive(false);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static string FormatValue(int value)
        {
            if (value > 0) return $"+{value}";
            return value.ToString(); // 0 hoặc âm giữ nguyên
        }
    }
}
