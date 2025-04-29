using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuCoinUI : MonoBehaviour
{
    public Image[] coinImages;         // Images for the collected coins
    public TMP_Text totalCoinText;     // Displays the total number of collected special coins

    void Start()
    {
        UpdateMainMenuUI();
    }

    void UpdateMainMenuUI()
    {
        // 🔢 Global coin counter
        int globalCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalSpecialCoins"); // 👈 Damit der Key ins JSON wandert

        if (totalCoinText != null)
        {
            totalCoinText.text = globalCoins.ToString();
        }

        // 🔍 Coin images aktivieren basierend auf PlayerPrefs
        for (int i = 0; i < coinImages.Length; i++)
        {
            // 💡 Beispiel: Level1.Coin1, Level2.Coin2, etc.
            string coinKey = $"Level{i + 1}.Coin{i + 1}";
            bool coinCollected = PlayerPrefs.HasKey(coinKey);
            PlayerPrefsKeyTracker.TrackKey(coinKey); // 👈 Auch diesen Key tracken

            if (coinImages[i] != null)
            {
                coinImages[i].gameObject.SetActive(coinCollected);
            }
        }
    }
}
