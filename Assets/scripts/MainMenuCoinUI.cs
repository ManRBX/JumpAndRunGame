using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuCoinUI : MonoBehaviour
{
    public Image[] coinImages;  // Bilder für die gesammelten Coins
    public TMP_Text totalCoinText;  // Zeigt nur die Anzahl gesammelter Spezialmünzen

    void Start()
    {
        UpdateMainMenuUI();
    }

    void UpdateMainMenuUI()
    {
        // Gesamtzahl der globalen Spezialmünzen laden
        int globalCoins = PlayerPrefs.GetInt("Global.SpecialCoins", 0);
        if (totalCoinText != null)
        {
            totalCoinText.text = globalCoins.ToString();  // Zeigt nur die Zahl, ohne zusätzlichen Text
        }

        // Bilder für Spezialmünzen je nach Fortschritt anzeigen
        for (int i = 0; i < coinImages.Length; i++)
        {
            string coinKey = $"Level0{i + 1}.Coin{i + 1}";
            bool coinCollected = PlayerPrefs.HasKey(coinKey);

            if (coinImages[i] != null)
            {
                coinImages[i].gameObject.SetActive(coinCollected); // Zeigt das Bild nur, wenn Münze gesammelt wurde
            }
        }
    }
}
