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

        // Prüfen, ob genug Special Coins vorhanden sind
        string coinKey = currentLevelName + "-special-coins"; // entspricht "Level01-special-coins" im PlayerPrefs
        int levelCoins = PlayerPrefs.GetInt(coinKey, 0);
        Debug.Log($"Special Coins für {targetID}: {levelCoins}/{achievementCoinsRequired}");

        if (levelCoins < achievementCoinsRequired)
        {
            Debug.Log($"❌ Du brauchst mindestens {achievementCoinsRequired} Special Coins, um hier weiterzukommen.");
            return;
        }

        // Ab hier reicht die Anzahl, Achievement auslösen und Szene wechseln
        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                AchievementProgressTracker.Instance?.OnLevelCompleted(targetID);
                Debug.Log($"✅ Achievement ausgelöst für {targetID}.");
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

        SceneManager.LoadScene(returnScene);
    }
}
