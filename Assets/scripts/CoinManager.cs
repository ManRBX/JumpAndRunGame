using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Settings")]
    public TMP_Text globalPointsText;  // Displays global points (10 digits)
    public TMP_Text levelPointsText;   // Displays level-specific points (10 digits)
    public TMP_Text globalCoinsText;   // Displays global coins (normal number)

    private int totalGlobalPoints;  // Total points across all levels
    private int levelPoints;        // Points for the current level
    private int globalCoins;        // Total number of collected coins
    private string currentLevel;    // Current level name

    private float updateInterval = 1f; // Interval for refreshing data (seconds)
    private float nextUpdateTime = 0f; // Time for the next update

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

        levelPoints += points;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);

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
        PlayerPrefs.Save();

        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0) + coins;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();

        UpdatePointsUI();

        Debug.Log($"💰 Coin collected! Total coins: {globalCoins}");
    }

    /// <summary>
    /// Loads saved data from PlayerPrefs.
    /// </summary>
    private void LoadData()
    {
        totalGlobalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
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
        {
            globalPointsText.text = totalGlobalPoints.ToString("D10"); // 10-digit format with leading zeros
        }

        if (levelPointsText != null)
        {
            levelPointsText.text = levelPoints.ToString("D10"); // 10-digit format with leading zeros
        }

        if (globalCoinsText != null)
        {
            globalCoinsText.text = globalCoins.ToString(); // Normal number without leading zeros
        }
    }
}
