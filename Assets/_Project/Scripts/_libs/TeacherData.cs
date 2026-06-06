using UnityEngine;

[CreateAssetMenu(fileName = "NewTeacherData", menuName = "Collection/Teacher Data", order = 1)]
public class TeacherData : ScriptableObject
{
    public int id;
    public string teacherName = "New Teacher";
    public int level = 1;
    public float seniority = 1.0f;
    
    [Header("Buffs")]
    [Tooltip("Hệ số nhân cho sản lượng Gold")]
    public float influenceGold = 1.0f;
    
    [Tooltip("Hệ số nhân cho Education")]
    public float influenceEducation = 1.0f;
    
    [Tooltip("Hệ số nhân cho Happiness")]
    public float influenceHappy = 1.0f;

    [Header("UI")]
    public Sprite avatar;
    public Sprite sexIcon;
    
    [Header("Descriptions")]
    [TextArea(1, 3)]
    public string descGold = "Tăng sản lượng Vàng thu được";
    [TextArea(1, 3)]
    public string descEducation = "Tăng chất lượng đào tạo";
    [TextArea(1, 3)]
    public string descHappy = "Tăng mức độ Hạnh phúc";
}
