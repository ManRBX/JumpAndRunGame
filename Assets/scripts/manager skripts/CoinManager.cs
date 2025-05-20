using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Settings")]
    public TMP_Text globalPointsText;  // Displays global points (10 digits)
    public TMP_Text levelPointsText;   // Displays level-specific points (10 digits)
    public TMP_Text globalCoinsText;   // Displays global coins (normal number)

    private int totalGlobalPoints;
    private int levelPoints;
    private int globalCoins;
    private string currentLevel;

    private float updateInterval = 1f;
    private float nextUpdateTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        LoadData();
        UpdatePointsUI();
    }

    private void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            RefreshPoints();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    /// <summary>
    /// Adds points and saves them.
    /// </summary>
    public void AddPoints(int points)
    {
        totalGlobalPoints += points;
        PlayerPrefs.SetInt("GlobalPoints", totalGlobalPoints);
        PlayerPrefsKeyTracker.TrackKey("GlobalPoints");

        levelPoints += points;
        string levelKey = $"{currentLevel}_Points";
        PlayerPrefs.SetInt(levelKey, levelPoints);
        PlayerPrefsKeyTracker.TrackKey(levelKey);

        PlayerPrefs.Save();
        UpdatePointsUI();

        Debug.Log($"🏆 Points received! Total: {totalGlobalPoints} | Level: {levelPoints}");
    }

    /// <summary>
    /// Adds coins and saves them.
    /// </summary>
    public void AddCoins(int coins)
    {
        globalCoins += coins;
        PlayerPrefs.SetInt("GlobalCoins", globalCoins);
        PlayerPrefsKeyTracker.TrackKey("GlobalCoins");

        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0) + coins;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefsKeyTracker.TrackKey("TotalCoins");

        PlayerPrefs.Save();
        UpdatePointsUI();
        FindAnyObjectByType<AchievementProgressTracker>()?.AddCoin();

        Debug.Log($"💰 Coin collected! Total coins: {globalCoins}");
    }

    /// <summary>
    /// Loads saved data from PlayerPrefs.
    /// </summary>
    private void LoadData()
    {
        totalGlobalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalPoints");

        string levelKey = $"{currentLevel}_Points";
        levelPoints = PlayerPrefs.GetInt(levelKey, 0);
        PlayerPrefsKeyTracker.TrackKey(levelKey);

        globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalCoins");
    }

    /// <summary>
    /// Refreshes saved values only if they have changed.
    /// </summary>
    private void RefreshPoints()
    {
        int savedGlobalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        int savedLevelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        int savedGlobalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);

        if (savedGlobalPoints != totalGlobalPoints || savedLevelPoints != levelPoints || savedGlobalCoins != globalCoins)
        {
            totalGlobalPoints = savedGlobalPoints;
            levelPoints = savedLevelPoints;
            globalCoins = savedGlobalCoins;

            UpdatePointsUI();
        }
    }

    /// <summary>
    /// Updates the UI displays for points and coins.
    /// </summary>
    public void UpdatePointsUI()
    {
        if (globalPointsText != null)
            globalPointsText.text = totalGlobalPoints.ToString("D10");

        if (levelPointsText != null)
            levelPointsText.text = levelPoints.ToString("D10");

        if (globalCoinsText != null)
            globalCoinsText.text = globalCoins.ToString();
    }

    /// <summary>
    /// Adds coins for achievements (separate from spendable global coins).
    /// </summary>
    public void AddAchievementCoin(int coins = 1)
    {
        int achievementCoins = PlayerPrefs.GetInt("AchievementCoins", 0);
        achievementCoins += coins;
        PlayerPrefs.SetInt("AchievementCoins", achievementCoins);
        PlayerPrefsKeyTracker.TrackKey("AchievementCoins");
        PlayerPrefs.Save();

        AchievementProgressTracker.Instance?.AddCoin(); // Optional: nur wenn du willst, dass dieser Zähler verwendet wird
        Debug.Log($"🏅 Achievement Coin collected! Total: {achievementCoins}");
    }
}
