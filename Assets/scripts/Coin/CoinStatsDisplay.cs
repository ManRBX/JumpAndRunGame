using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CoinStatsDisplay : MonoBehaviour
{
    [Header("🔢 UI Elements (Mehrere erlaubt)")]
    public List<TMP_Text> globalPointsTexts;
    public List<TMP_Text> levelPointsTexts;
    public List<TMP_Text> globalCoinsTexts;
    public List<TMP_Text> levelCoinsTexts;

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
        foreach (var txt in globalPointsTexts)
            if (txt != null) txt.text = globalPoints.ToString();

        foreach (var txt in levelPointsTexts)
            if (txt != null) txt.text = levelPoints.ToString();

        foreach (var txt in globalCoinsTexts)
            if (txt != null) txt.text = globalCoins.ToString();

        foreach (var txt in levelCoinsTexts)
            if (txt != null) txt.text = levelCoins.ToString();
    }
}
