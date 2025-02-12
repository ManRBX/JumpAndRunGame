using UnityEngine;
using TMPro;

public class CoinStatsDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text globalPointsText;  // Displays only the global point total
    public TMP_Text levelPointsText;   // Displays only the level-specific point total
    public TMP_Text globalCoinsText;   // Displays only the global coin count
    public TMP_Text levelCoinsText;    // Displays only the level-specific coin count

    void Start()
    {
        UpdatePointStats();
    }

    public void UpdatePointStats()
    {
        // Retrieve global values
        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        int globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);

        // Retrieve level-specific values
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        int levelCoins = PlayerPrefs.GetInt($"{currentLevel}_Coins", 0);

        // Display global points
        if (globalPointsText != null)
        {
            globalPointsText.text = globalPoints.ToString();
        }

        // Display level-specific points
        if (levelPointsText != null)
        {
            levelPointsText.text = levelPoints.ToString();
        }

        // Display global coins
        if (globalCoinsText != null)
        {
            globalCoinsText.text = globalCoins.ToString();
        }

        // Display level-specific coins
        if (levelCoinsText != null)
        {
            levelCoinsText.text = levelCoins.ToString();
        }
    }
}
