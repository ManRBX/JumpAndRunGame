using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinStatsDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text globalPointsText;
    public TMP_Text levelPointsText;
    public TMP_Text globalCoinsText;
    public TMP_Text levelCoinsText;

    void Start()
    {
        UpdatePointStats();
    }

    public void UpdatePointStats()
    {
        // Globalwerte abrufen und tracken
        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalPoints");

        int globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalCoins");

        // Level-spezifische Werte abrufen und tracken
        string currentLevel = SceneManager.GetActiveScene().name;

        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        PlayerPrefsKeyTracker.TrackKey($"{currentLevel}_Points");

        int levelCoins = PlayerPrefs.GetInt($"{currentLevel}_Coins", 0);
        PlayerPrefsKeyTracker.TrackKey($"{currentLevel}_Coins");

        // UI aktualisieren
        if (globalPointsText != null)
            globalPointsText.text = globalPoints.ToString();

        if (levelPointsText != null)
            levelPointsText.text = levelPoints.ToString();

        if (globalCoinsText != null)
            globalCoinsText.text = globalCoins.ToString();

        if (levelCoinsText != null)
            levelCoinsText.text = levelCoins.ToString();
    }
}
