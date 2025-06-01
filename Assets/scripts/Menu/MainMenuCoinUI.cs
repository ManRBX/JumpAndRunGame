using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuCoinUI : MonoBehaviour
{
    [Header("💰 Coin-Anzeige für jedes Level")]
    public List<Image> coinImages;         // Eine Image pro Coin, frei erweiterbar im Inspector

    [Header("🔢 Gesamtanzahl der Coins")]
    public List<TMP_Text> totalCoinTexts;  // Beliebig viele Textfelder, die den Gesamtstand anzeigen

    void Start()
    {
        UpdateMainMenuUI();
    }

    void UpdateMainMenuUI()
    {
        // 🔢 Global coin counter laden + tracken
        int globalCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalSpecialCoins");

        // Alle Texte aktualisieren
        foreach (TMP_Text txt in totalCoinTexts)
        {
            if (txt != null)
                txt.text = globalCoins.ToString();
        }

        // 🔍 Einzelne Coins anzeigen, je nachdem ob sie gesammelt wurden
        for (int i = 0; i < coinImages.Count; i++)
        {
            string coinKey = $"Level{i + 1}.Coin{i + 1}";
            bool coinCollected = PlayerPrefs.HasKey(coinKey);
            PlayerPrefsKeyTracker.TrackKey(coinKey);

            if (coinImages[i] != null)
                coinImages[i].gameObject.SetActive(coinCollected);
        }
    }
}
