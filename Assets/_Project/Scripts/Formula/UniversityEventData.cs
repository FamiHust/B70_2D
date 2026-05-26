using System.Collections.Generic;
using UnityEngine;

namespace B70.Balance
{
    [System.Serializable]
    public class EventOption
    {
        public string title;
        [TextArea(2, 4)]
        public string description;

        [Header("Effects")]
        [Tooltip("Sự thay đổi về Gold (+ để nhận, - để trừ)")]
        public int goldModifier = 0;

        [Tooltip("Sự thay đổi về Happiness (+ hoặc -)")]
        public float happinessModifier = 0f;

        [Tooltip("Sự thay đổi về Education/Academic (+ hoặc -)")]
        public float educationModifier = 0f;
    }
    [CreateAssetMenu(fileName = "New Event Data", menuName = "B70/Balance/Event Data")]
    public class UniversityEventData : ScriptableObject
    {
        public string eventID;
        public string eventName;

        [TextArea(2, 4)]
        public string description;

        [Header("Visuals")]
        public Sprite eventSprite;

        [Range(0f, 1f)]
        [Tooltip("Xác suất xảy ra event trong một học kỳ (Ví dụ: 0.35 = 35%)")]
        public float triggerProbability = 0.35f;


        [Header("Options")]
        public List<EventOption> options = new List<EventOption>();
    }
}
