using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelEnd : MonoBehaviour
{
    [Header("🔗 Level Infos")]
    public string nextLevelName;        // z. B. "Level02"
    public string currentLevelName;     // z. B. "Level01"
    public string returnScene = "Menu"; // z. B. "MainMenu"

    [Header("💰 Anforderungen")]
    public int requiredSpecialCoins = 5;

    [Header("📊 UI")]
    public TMP_Text coinProgressText;

    private void Start()
    {
        // Beim Start markieren: besucht + freigeschaltet
        if (currentLevelName == "Level01")
        {
            PlayerPrefs.SetInt(currentLevelName + "_Unlocked", 1);
            PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Unlocked");
        }

        PlayerPrefs.SetInt(currentLevelName + "_Visited", 1);
        PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Visited");

        PlayerPrefs.SetInt(currentLevelName + "_Completed", 1);
        PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Completed");

        PlayerPrefs.Save();

        if (coinProgressText != null)
            coinProgressText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        int collectedGlobalSpecialCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);
        PlayerPrefsKeyTracker.TrackKey("GlobalSpecialCoins");

        if (collectedGlobalSpecialCoins >= requiredSpecialCoins)
        {
            // 🎯 Level abgeschlossen
            PlayerPrefs.SetInt(currentLevelName + "_Completed", 1);
            PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Completed");
            Debug.Log(currentLevelName + " completed!");

            // 🚀 Steam-Achievement: Level abgeschlossen
            AchievementProgressTracker.Instance?.OnLevelCompleted(currentLevelName);

            if (!string.IsNullOrEmpty(nextLevelName))
            {
                // 🔓 Nächstes Level freischalten
                PlayerPrefs.SetInt(nextLevelName + "_Unlocked", 1);
                PlayerPrefsKeyTracker.TrackKey(nextLevelName + "_Unlocked");
                Debug.Log(nextLevelName + " unlocked!");

                // 🚀 Steam-Achievement: Nächstes Level freigeschaltet
                AchievementProgressTracker.Instance?.OnLevelUnlocked(nextLevelName);
            }

            PlayerPrefs.Save();

            // 💾 JSON speichern
            PlayerPrefsSaver saver = FindObjectOfType<PlayerPrefsSaver>();
            if (saver != null)
            {
                saver.SavePrefsToJson();
                Debug.Log("📂 PlayerPrefs wurden zusätzlich im JSON gespeichert.");
            }

            // 🔁 Zurück ins Menü oder nächste Szene
            SceneManager.LoadScene(returnScene);
        }
        else
        {
            Debug.Log($"❌ Nicht genug Spezial-Coins! Benötigt: {requiredSpecialCoins}, Aktuell: {collectedGlobalSpecialCoins}");
        }

        // 🔔 Coin-Anzeige zeigen
        ShowCoinProgress();
    }

    void UpdateCoinProgressUI()
    {
        int collectedGlobalSpecialCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);

        if (coinProgressText != null)
            coinProgressText.text = $"{collectedGlobalSpecialCoins}/{requiredSpecialCoins}";
    }

    void ShowCoinProgress()
    {
        UpdateCoinProgressUI();

        if (coinProgressText != null)
            coinProgressText.gameObject.SetActive(true);

        StartCoroutine(HideCoinProgress());
    }

    IEnumerator HideCoinProgress()
    {
        yield return new WaitForSeconds(5f);

        if (coinProgressText != null)
            coinProgressText.gameObject.SetActive(false);
    }
}
