using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI")]
    public TMP_Text globalPointsText;   // 10-stellig formatiert
    public TMP_Text levelPointsText;    // 10-stellig formatiert
    public TMP_Text globalCoinsText;    // normale Zahl

    [Header("Einstellungen")]
    [Tooltip("Hält den Manager über Szenenwechsel am Leben.")]
    public bool persistAcrossScenes = false;

    [Tooltip("Wie oft gespeicherte Werte neu eingelesen werden (Sekunden).")]
    public float updateInterval = 1f;

    private int totalGlobalPoints;
    private int levelPoints;
    private int globalCoins;
    private string currentLevel;

    private float nextUpdateTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (persistAcrossScenes) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLevel = SceneManager.GetActiveScene().name;
        LoadData();
        UpdatePointsUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevel = scene.name;
        levelPoints = PlayerPrefs.GetInt(LevelKey(currentLevel), 0);
        UpdatePointsUI();
    }

    private void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            RefreshFromStorage();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    // ----------------- Öffentliche API -----------------

    /// <summary>Erhöht Punkte (global + level) und speichert sie.</summary>
    public void AddPoints(int points)
    {
        if (points == 0) return;

        totalGlobalPoints += points;
        PlayerPrefs.SetInt("GlobalPoints", totalGlobalPoints);

        levelPoints += points;
        PlayerPrefs.SetInt(LevelKey(currentLevel), levelPoints);

        PlayerPrefs.Save();
        UpdatePointsUI();
        // Debug.Log($"Points: global={totalGlobalPoints} | {currentLevel}={levelPoints}");
    }

    /// <summary>Erhöht Coins (global) und speichert sie. Zählt zusätzlich einen einfachen Gesamt‑Counter.</summary>
    public void AddCoins(int coins)
    {
        if (coins == 0) return;

        globalCoins += coins;
        PlayerPrefs.SetInt("GlobalCoins", globalCoins);

        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0) + coins;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);

        PlayerPrefs.Save();
        UpdatePointsUI();
        // Debug.Log($"Coins: global={globalCoins} | total(all-time)={totalCoins}");
    }

    // ----------------- Intern -----------------

    private void LoadData()
    {
        totalGlobalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        levelPoints = PlayerPrefs.GetInt(LevelKey(currentLevel), 0);
        globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
    }

    private void RefreshFromStorage()
    {
        int gp = PlayerPrefs.GetInt("GlobalPoints", 0);
        int lp = PlayerPrefs.GetInt(LevelKey(currentLevel), 0);
        int gc = PlayerPrefs.GetInt("GlobalCoins", 0);

        if (gp != totalGlobalPoints || lp != levelPoints || gc != globalCoins)
        {
            totalGlobalPoints = gp;
            levelPoints = lp;
            globalCoins = gc;
            UpdatePointsUI();
        }
    }

    private void UpdatePointsUI()
    {
        if (globalPointsText != null)
            globalPointsText.text = totalGlobalPoints.ToString("D10");

        if (levelPointsText != null)
            levelPointsText.text = levelPoints.ToString("D10");

        if (globalCoinsText != null)
            globalCoinsText.text = globalCoins.ToString();
    }

    private static string LevelKey(string levelName) => levelName + "_Points";
}
