using System.Collections.Generic;
using UnityEngine;

namespace B70.Balance
{
    [System.Serializable]
    public class RandomEventConfig
    {
        public string eventID;
        public string eventName;
        [TextArea(2, 4)]
        public string description;
        
        [Range(0f, 1f)]
        [Tooltip("Xác suất xảy ra event trong một học kỳ (Ví dụ: 0.35 = 35%)")]
        public float triggerProbability = 0.35f; 
        
        [Header("Effects")]
        [Tooltip("Số Gold bị trừ khi event xảy ra (nếu không đủ tiền, event sẽ bị bỏ qua)")]
        public int goldCost = 0;
        
        [Tooltip("Sự thay đổi về Happiness (+ hoặc -)")]
        public float happinessModifier = 0f;
        
        [Tooltip("Sự thay đổi về Education/Academic (+ hoặc -)")]
        public float educationModifier = 0f;
    }

    /// <summary>
    /// Quản lý việc roll ngẫu nhiên và áp dụng các Event trong game.
    /// Có thể đính kèm vào bất kỳ GameObject nào (như GameManager hoặc SceneManager object).
    /// </summary>
    public class UniversityEventManager : MonoBehaviour
    {
        public static UniversityEventManager instance;

        [Header("Event Database")]
        [Tooltip("Danh sách các sự kiện ngẫu nhiên có thể xảy ra trong kỳ")]
        public List<RandomEventConfig> availableEvents = new List<RandomEventConfig>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Gọi hàm này tại thời điểm cuối kỳ (vd: trong SceneManager.CompleteSemester).
        /// Trả về danh sách các event đã thực sự xảy ra để hiển thị lên Popup UI.
        /// </summary>
        public List<RandomEventConfig> RollAndApplyRandomEvents()
        {
            List<RandomEventConfig> triggeredEvents = new List<RandomEventConfig>();

            if (SceneManager.instance == null) return triggeredEvents;

            foreach (var evt in availableEvents)
            {
                // Tung xúc xắc ngẫu nhiên từ 0.0 đến 1.0
                float roll = Random.Range(0f, 1f);

                if (roll <= evt.triggerProbability)
                {
                    // Nếu event tốn tiền, kiểm tra xem người chơi có đủ Gold không
                    if (evt.goldCost > 0)
                    {
                        if (SceneManager.instance.numberOfGoldInStorage < evt.goldCost)
                        {
                            // Không đủ tiền -> bỏ qua event này
                            continue;
                        }
                        
                        // Trừ tiền ngay lập tức
                        SceneManager.instance.numberOfGoldInStorage -= evt.goldCost;
                    }
                    
                    // Áp dụng thay đổi vào chỉ số H và A của trường
                    SceneManager.instance.numberOfHappyInStorage = Mathf.Clamp(
                        SceneManager.instance.numberOfHappyInStorage + Mathf.RoundToInt(evt.happinessModifier), 0, 100);
                        
                    SceneManager.instance.numberOfEducationInStorage = Mathf.Clamp(
                        SceneManager.instance.numberOfEducationInStorage + Mathf.RoundToInt(evt.educationModifier), 0, 100);
                    
                    triggeredEvents.Add(evt);
                    
                    Debug.Log($"[Event Triggered] {evt.eventName} | H: {(evt.happinessModifier >= 0 ? "+" : "")}{evt.happinessModifier}, E: {(evt.educationModifier >= 0 ? "+" : "")}{evt.educationModifier}, Gold: -{evt.goldCost}");
                }
            }

            // Lưu và cập nhật lại giao diện ngay sau khi chạy event xong
            if (triggeredEvents.Count > 0)
            {
                SceneManager.instance.SaveResources();
                SceneManager.instance.RefreshResourceUIs("gold");
                // Lưu ý: Nếu bạn có UI cho Happy/Education thì cần gọi RefreshResourceUIs cho chúng ở đây.
            }

            return triggeredEvents;
        }
    }
}
