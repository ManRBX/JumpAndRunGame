using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuCoinUI : MonoBehaviour
{
    public Image[] coinImages;  // Images for the collected coins
    public TMP_Text totalCoinText;  // Displays the total number of collected special coins

    void Start()
    {
        UpdateMainMenuUI();
    }

    void UpdateMainMenuUI()
    {
        // Load the total number of global special coins
        int globalCoins = PlayerPrefs.GetInt("Global.SpecialCoins", 0);
        if (totalCoinText != null)
        {
            totalCoinText.text = globalCoins.ToString();  // Displays only the number, without additional text
        }

        // Show images for special coins based on progress
        for (int i = 0; i < coinImages.Length; i++)
        {
            string coinKey = $"Level0{i + 1}.Coin{i + 1}";
            bool coinCollected = PlayerPrefs.HasKey(coinKey);

            if (coinImages[i] != null)
            {
                coinImages[i].gameObject.SetActive(coinCollected); // Only shows the image if the coin is collected
            }
        }
    }
}
