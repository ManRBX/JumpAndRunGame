using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelEnd : MonoBehaviour
{
    public enum TriggerType
    {
        LevelComplete,
        LevelUnlock,
        SecretFound
    }

    [Header("🔗 Level Infos")]
    public string nextLevelName;
    public string currentLevelName;
    public string returnScene = "Menu";

    [Header("🏆 Achievement Settings")]
    public TriggerType triggerType;
    [Tooltip("z. B. Level01 und 'Level01-special-coins'")]
    public string targetID;
    [Tooltip("Minimale Special Coins, um durchzukommen und Achievement auszulösen.")]
    public int achievementCoinsRequired = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        string coinKey = currentLevelName + "-special-coins";
        int levelCoins = PlayerPrefs.GetInt(coinKey, 0);
        Debug.Log($"Special Coins für {targetID}: {levelCoins}/{achievementCoinsRequired}");

        if (levelCoins < achievementCoinsRequired)
        {
            Debug.Log($"❌ Du brauchst mindestens {achievementCoinsRequired} Special Coins, um hier weiterzukommen.");
            return;
        }

        // ✅ Erfolgreich – Achievement + Freischalten
        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                AchievementProgressTracker.Instance?.OnLevelCompleted(targetID);
                Debug.Log($"✅ Achievement ausgelöst für {targetID}.");

                // 👉 Level freischalten
                if (!string.IsNullOrEmpty(nextLevelName))
                {
                    string unlockKey = nextLevelName + "_Unlocked";
                    PlayerPrefs.SetInt(unlockKey, 1);
                    PlayerPrefs.Save();
                    Debug.Log($"🔓 Freigeschaltet: {nextLevelName} ({unlockKey})");
                }
                break;

            case TriggerType.LevelUnlock:
                AchievementProgressTracker.Instance?.OnLevelUnlocked(targetID);
                Debug.Log($"✅ LevelUnlock Achievement ausgelöst für {targetID}.");
                break;

            case TriggerType.SecretFound:
                AchievementProgressTracker.Instance?.OnSecretFound(targetID);
                Debug.Log($"✅ SecretFound Achievement ausgelöst für {targetID}.");
                break;
        }

        // Zurück ins Menü oder nächste Szene
        SceneManager.LoadScene(returnScene);
    }
}
