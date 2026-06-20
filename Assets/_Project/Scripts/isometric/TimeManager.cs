using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("Semester Settings")]
    public float semesterDuration = 60f; // Thời gian một kỳ học (giây)
    public float timeRemaining;
    public bool isPaused = false;

    public event Action OnSemesterEnd;

    public bool hasFinishedFinalTutorial = false;
    public bool isTutorialTimeRunning = false;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SaveTimer();
            instance = null;
        }
    }

    private void Start()
    {
        LoadTimer();
    }

    private void OnApplicationQuit()
    {
        SaveTimer();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveTimer();
        }
    }

    public void SaveTimer()
    {
        PlayerPrefs.SetFloat("timeRemaining", timeRemaining);
        PlayerPrefs.SetInt("hasFinishedFinalTutorial", hasFinishedFinalTutorial ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadTimer()
    {
        // Nếu chưa có dữ liệu lưu, mặc định dùng semesterDuration
        timeRemaining = PlayerPrefs.GetFloat("timeRemaining", semesterDuration);
        
        // Nếu lỡ load lên thấy <= 0 (đã kết thúc nhưng chưa kịp reset), 
        // thì reset lại để tránh kẹt loop
        if (timeRemaining <= 0)
        {
            timeRemaining = semesterDuration;
        }

        hasFinishedFinalTutorial = PlayerPrefs.GetInt("hasFinishedFinalTutorial", 0) == 1;
        
        // Mặc định khởi tạo luôn pause (vì game sẽ bắt đầu ở Menu).
        // Timer sẽ được unpause sau khi người chơi bấm Play và các màn hình chờ đóng lại (thông qua UIManager.CheckWindowsAfterClose).
        isPaused = true;
    }

    private void Update()
    {
        if (isPaused) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                TriggerSemesterEnd();
            }
        }
    }

    public void ResetTimer()
    {
        timeRemaining = semesterDuration;
        isPaused = true;
        SaveTimer();
    }

    private void TriggerSemesterEnd()
    {
        Debug.Log("[TimeManager] Semester time is up!");
        if (SceneManager.instance != null)
        {
            SceneManager.instance.CompleteSemester();
        }
        OnSemesterEnd?.Invoke();
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
