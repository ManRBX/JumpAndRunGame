using UnityEngine;
using UnityEngine.UI;

public class SpecialCoinUI : MonoBehaviour
{
    public Image[] coinImages;   // Bilder für die speziellen Coins
    private string levelName;

    void Start()
    {
        levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UpdateCoinUI();  // Aktualisiere die UI beim Start
    }

    // Münze auf Basis des Index in der UI anzeigen
    public void CollectCoin(int coinIndex)
    {
        if (coinIndex < 0 || coinIndex >= coinImages.Length) return;

        // Volle Sichtbarkeit aktivieren
        coinImages[coinIndex].color = new Color(1, 1, 1, 1);

        // Münze speichern
        string coinKey = $"{levelName}.Coin{coinIndex}";
        PlayerPrefs.SetInt(coinKey, 1);
        PlayerPrefs.Save();
    }

    // UI wird beim Levelstart aktualisiert
    public void UpdateCoinUI()
    {
        for (int i = 0; i < coinImages.Length; i++)
        {
            string coinKey = $"{levelName}.Coin{i}";
            if (PlayerPrefs.HasKey(coinKey))
            {
                // Volle Sichtbarkeit, wenn eingesammelt
                coinImages[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                // Halbtransparent, wenn nicht eingesammelt
                coinImages[i].color = new Color(1, 1, 1, 0.1f);
            }
        }
    }
}
