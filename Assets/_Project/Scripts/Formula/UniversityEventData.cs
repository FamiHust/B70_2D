using UnityEngine;

namespace B70.Balance
{
    [CreateAssetMenu(fileName = "New Event Data", menuName = "B70/Balance/Event Data")]
    public class UniversityEventData : ScriptableObject
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
}
