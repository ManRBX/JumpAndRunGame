using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Settings")]
    public TMP_Text globalPointsText;  // Anzeige der globalen Punkte (10 Stellen)
    public TMP_Text levelPointsText;   // Anzeige der Level-spezifischen Punkte (10 Stellen)
    public TMP_Text globalCoinsText;   // Anzeige der globalen Coins (Normale Zahl)

    private int totalGlobalPoints;  // Gesamtpunkte über alle Level
    private int levelPoints;        // Punkte nur für dieses Level
    private int globalCoins;        // Gesamtzahl der gesammelten Coins
    private string currentLevel;    // Aktuelles Level

    private float updateInterval = 1f; // Aktualisierungsintervall in Sekunden
    private float nextUpdateTime = 0f; // Zeitpunkt der nächsten Aktualisierung

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

        // Punkte und Coins aus PlayerPrefs laden
        totalGlobalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);

        // UI sofort aktualisieren
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

    public void AddPoints(int points)
    {
        totalGlobalPoints += points;
        PlayerPrefs.SetInt("GlobalPoints", totalGlobalPoints);

        levelPoints += points;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);

        PlayerPrefs.Save();
        UpdatePointsUI();
    }

    public void AddCoins(int coins)
    {
        globalCoins += coins;
        PlayerPrefs.SetInt("GlobalCoins", globalCoins);
        PlayerPrefs.Save();

        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0) + coins;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();

        UpdatePointsUI();
    }

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

    private void UpdatePointsUI()
    {
        if (globalPointsText != null)
        {
            globalPointsText.text = totalGlobalPoints.ToString("D10"); // 10-stellig mit führenden Nullen
        }

        if (levelPointsText != null)
        {
            levelPointsText.text = levelPoints.ToString("D10"); // 10-stellig mit führenden Nullen
        }

        if (globalCoinsText != null)
        {
            globalCoinsText.text = globalCoins.ToString(); // Normale Zahl ohne führende Nullen
        }
    }
}
