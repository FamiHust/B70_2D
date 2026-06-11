using UnityEngine;
using B70.Leaderboard;

/// <summary>
/// Tester đơn giản — dùng phím tắt để test leaderboard mà không cần UI.
/// Xem kết quả trong Console.
/// XÓA file này trước khi push production.
/// </summary>
public class LeaderboardTester : MonoBehaviour
{
    [Header("Fake Player Stats (để test formula)")]
    public float fakeHappy = 60f;
    public float fakeEducation = 70f;
    public int fakeLevel = 2;
    public int fakeStudents = 400;

    [Header("Phím tắt")]
    [Tooltip("Bắt đầu session mới")]
    public KeyCode keyStart = KeyCode.F1;
    [Tooltip("Giả lập 1 semester kết thúc")]
    public KeyCode keyNextSemester = KeyCode.F2;
    [Tooltip("In bảng xếp hạng ra Console")]
    public KeyCode keyPrint = KeyCode.F3;
    [Tooltip("Reset session")]
    public KeyCode keyReset = KeyCode.F4;
    [Tooltip("Test formula với fake stats ở trên (không cần SceneManager)")]
    public KeyCode keyTestFormula = KeyCode.F5;

    private void Update()
    {
        if (Input.GetKeyDown(keyStart)) StartSession();
        if (Input.GetKeyDown(keyNextSemester)) SimulateSemester();
        if (Input.GetKeyDown(keyPrint)) PrintLeaderboard();
        if (Input.GetKeyDown(keyReset)) ResetSession();
        if (Input.GetKeyDown(keyTestFormula)) TestFormula();
    }

    private void StartSession()
    {
        if (LeaderboardManager.instance == null)
        {
            Debug.LogError("[Tester] LeaderboardManager không tìm thấy trong scene!");
            return;
        }
        LeaderboardManager.instance.StartSession();
        Debug.Log("[Tester] Session started. Nhấn F2 để giả lập semester, F3 để xem bảng.");
    }

    private void SimulateSemester()
    {
        if (LeaderboardManager.instance == null) return;

        // Nếu SceneManager có sẵn thì dùng real data
        // Nếu không (test scene riêng) thì patch fake stats vào SceneManager tạm
        if (SceneManager.instance != null)
        {
            SceneManager.instance.numberOfHappyInStorage = (int)fakeHappy;
            SceneManager.instance.numberOfEducationInStorage = (int)fakeEducation;
            SceneManager.instance.currentLevel = fakeLevel;
            SceneManager.instance.numberOfStudentInStorage = fakeStudents;
        }

        LeaderboardManager.instance.OnSemesterCompleted();
        PrintLeaderboard();
    }

    private void PrintLeaderboard()
    {
        if (LeaderboardManager.instance == null) return;

        var entries = LeaderboardManager.instance.GetRankedEntries();
        if (entries == null || entries.Count == 0)
        {
            Debug.Log("[Tester] Leaderboard trống — hãy StartSession trước (F1).");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("===== LEADERBOARD =====");
        foreach (var e in entries)
        {
            string tag = e.isPlayer ? " ◄ YOU" : "";
            sb.AppendLine($"  #{e.rank}  {e.displayName,-15}  {e.score,8:F0} pts{tag}");
        }
        sb.AppendLine($"=======================");
        sb.AppendLine($"Player rank: {LeaderboardManager.instance.GetPlayerRank()} / {LeaderboardManager.instance.GetTotalParticipants()}");
        Debug.Log(sb.ToString());
    }

    private void ResetSession()
    {
        if (LeaderboardManager.instance == null) return;
        LeaderboardManager.instance.ResetSession();
        Debug.Log("[Tester] Session reset xong.");
    }

    private void TestFormula()
    {
        float score = LeaderboardScoreFormula.Calculate(fakeHappy, fakeEducation, fakeLevel, fakeStudents);
        Debug.Log($"[Tester] Formula test — Happy:{fakeHappy} Edu:{fakeEducation} Level:{fakeLevel} Students:{fakeStudents} → Score: {score:F0}");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 160));
        GUILayout.Label("=== Leaderboard Tester ===");
        GUILayout.Label("F1 = Start Session");
        GUILayout.Label("F2 = Simulate Semester (dùng fake stats)");
        GUILayout.Label("F3 = Print Leaderboard");
        GUILayout.Label("F4 = Reset Session");
        GUILayout.Label("F5 = Test Formula Only");
        GUILayout.EndArea();
    }
}