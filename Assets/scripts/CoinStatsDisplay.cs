using UnityEngine;
using TMPro;

public class CoinStatsDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text globalPointsText;  // Zeigt nur die globale Punktzahl
    public TMP_Text levelPointsText;   // Zeigt nur die Punktzahl des Levels
    public TMP_Text globalCoinsText;   // Zeigt nur die globale Münzenanzahl
    public TMP_Text levelCoinsText;    // Zeigt nur die Münzen des Levels

    void Start()
    {
        UpdatePointStats();
    }

    public void UpdatePointStats()
    {
        // Globale Werte abrufen
        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        int globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);

        // Level-spezifische Werte abrufen
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        int levelCoins = PlayerPrefs.GetInt($"{currentLevel}_Coins", 0);

        // Globale Punkte anzeigen
        if (globalPointsText != null)
        {
            globalPointsText.text = globalPoints.ToString();
        }

        // Level-spezifische Punkte anzeigen
        if (levelPointsText != null)
        {
            levelPointsText.text = levelPoints.ToString();
        }

        // Globale Münzen anzeigen
        if (globalCoinsText != null)
        {
            globalCoinsText.text = globalCoins.ToString();
        }

        // Level-spezifische Münzen anzeigen
        if (levelCoinsText != null)
        {
            levelCoinsText.text = levelCoins.ToString();
        }
    }
}
