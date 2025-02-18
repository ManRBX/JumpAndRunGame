using UnityEngine;
using UnityEngine.UI;

public class SpecialCoinUI : MonoBehaviour
{
    public Image[] coinImages;   // Images for the special coins
    private string levelName;

    void Start()
    {
        levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UpdateCoinUI();  // Update the UI at the start
    }

    // Display the coin in the UI based on its index
    public void CollectCoin(int coinIndex)
    {
        if (coinIndex < 0 || coinIndex >= coinImages.Length) return;

        // Enable full visibility
        coinImages[coinIndex].color = new Color(1, 1, 1, 1);

        // Save the collected coin
        string coinKey = $"{levelName}.Coin{coinIndex}";
        PlayerPrefs.SetInt(coinKey, 1);
        PlayerPrefs.Save();
    }

    // Update the UI at the start of the level
    public void UpdateCoinUI()
    {
        for (int i = 0; i < coinImages.Length; i++)
        {
            string coinKey = $"{levelName}.Coin{i}";
            if (PlayerPrefs.HasKey(coinKey))
            {
                // Full visibility if collected
                coinImages[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                // Semi-transparent if not collected
                coinImages[i].color = new Color(1, 1, 1, 0.1f);
            }
        }
    }
}
