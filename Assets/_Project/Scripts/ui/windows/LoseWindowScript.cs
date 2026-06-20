using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseWindowScript : WindowScript
{
    public static LoseWindowScript instance;
    public static bool showMenuAfterReset = false;

    private void Awake()
    {
        instance = this;
    }

    public void Setup()
    {
        // Optional lose window initialization.
    }

    public void OnClickResetProgress()
    {
        ResetProgress();
    }

    public void ResetProgress()
    {
        // ── 1. Clear all persistent data ──────────────────────────────────
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        ResetDbJsonFile();
        CreateDefaultDbJsonFile();

        // ── 2. Reset all resource values to default ──────────────────────────
        if (global::SceneManager.instance != null)
        {
            global::SceneManager.instance.numberOfGoldInStorage = 200;
            global::SceneManager.instance.numberOfDiamondsInStorage = 10;
            global::SceneManager.instance.numberOfStudentInStorage = 850;
            global::SceneManager.instance.numberOfHappyInStorage = 12;
            global::SceneManager.instance.numberOfEducationInStorage = 12;
            
            // Reset capacity to initial values
            global::SceneManager.instance.goldStorageCapacity = 10000;
            global::SceneManager.instance.diamondStorageCapacity = 10;
            global::SceneManager.instance.studentStorageCapacity = 850;
            global::SceneManager.instance.happyStorageCapacity = 100;
            global::SceneManager.instance.educationStorageCapacity = 100;
            
            // Reset semester and level
            global::SceneManager.instance.currentSemester = 1;
            global::SceneManager.instance.currentLevel = 1;
            global::SceneManager.instance.levelProgress = 0;
            global::SceneManager.instance.totalSpawnedNPCs = 0;
        }

        // ── 3. Reset time & skip tutorial on replay ────────────────────────
        if (TimeManager.instance != null)
        {
            TimeManager.instance.hasFinishedFinalTutorial = true;
            TimeManager.instance.isTutorialTimeRunning = false;
            TimeManager.instance.SetPaused(true);
            TimeManager.instance.ResetTimer();
        }

        // Mark tutorial as finished so it won't replay
        PlayerPrefs.SetInt("hasFinishedFinalTutorial", 1);
        PlayerPrefs.Save();

        if (global::SceneManager.instance != null)
        {
            global::SceneManager.instance.isTutorialActive = false;
        }

        // ── 4. Reset DataBaseManager ──────────────────────────────────
        if (DataBaseManager.instance != null)
        {
            DataBaseManager.instance.EnsureGameDataFileExists();
            Debug.Log("[LoseWindow] Reloaded DataBaseManager data");
        }

        // ── 5. Clear all scene buildings ─────────────────────────────────────
        if (global::SceneManager.instance != null)
        {
            global::SceneManager.instance.ClearScene();
            global::SceneManager.instance.SaveResources();
        }

        // ── 6. Close all windows and reload/reset the scene ─────────────────
        if (UIManager.instance != null)
        {
            UIManager.instance.CloseAllWindows();
        }

        // ── 7. Navigate back to menu via scene reload ────────────────────────
        showMenuAfterReset = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        Debug.Log("[LoseWindow] Game reset to initial state. Scene reloading...");
    }
    private string GetDatabaseFilePath()
    {
        string gameDataFilePath = "/StreamingAssets/db.json";
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
        {
            return Application.dataPath + gameDataFilePath;
        }
        else
        {
            return Application.persistentDataPath + gameDataFilePath;
        }
    }

    private void CreateDefaultDbJsonFile()
    {
        try
        {
            // Create default game data: empty buildings, no missions
            string defaultSceneJson = "{\"sceneData\":{\"items\":[]},\"claimedMissionIds\":[]}";
            string filePath = GetDatabaseFilePath();
            
            // Ensure directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            File.WriteAllText(filePath, defaultSceneJson);
            Debug.Log($"[LoseWindow] Created default db.json at: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[LoseWindow] Failed to create default db.json: {ex.Message}");
        }
    }

    private void ResetDbJsonFile()
    {
        try
        {
            string filePath = GetDatabaseFilePath();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[LoseWindow] Deleted db.json at: {filePath}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[LoseWindow] Failed to reset db.json: {ex.Message}");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
