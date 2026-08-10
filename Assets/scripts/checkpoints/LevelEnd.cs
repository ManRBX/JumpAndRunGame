using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [Tooltip("Nur bei LevelComplete verwendet. Wenn leer, wird returnScene geladen.")]
    public string nextLevelName;

    [Tooltip("PlayerPrefs-Key zum Freischalten des nächsten Levels, z.B. Level02_Unlocked.")]
    public string nextLevelUnlockKey;

    [Tooltip("Name des aktuellen Levels für Speicher-Keys. Wenn leer, wird der aktuelle Szenenname verwendet.")]
    public string currentLevelName;

    [Tooltip("Fallback-Szene, zum Beispiel Menu.")]
    public string returnScene = "Menu";

    [Tooltip("Nach LevelComplete das nächste Level laden. Sonst wird returnScene geladen.")]
    public bool loadNextLevelOnComplete = true;

    [Header("🎯 Trigger Modus")]
    public TriggerType triggerType = TriggerType.LevelComplete;

    [Tooltip("Kompletter PlayerPrefs-Key, z.B. Level01_Completed oder SecretFound_Room01.")]
    public string targetID;

    [Header("🪙 Coin Gate")]
    [Tooltip("Minimale Special Coins zum Abschließen. 0 deaktiviert die Prüfung.")]
    public int achievementCoinsRequired = 0;

    [Tooltip("Überschreibt den automatisch erzeugten Coin-Key.")]
    public string coinKeyOverride;

    [Header("🎬 Feedback")]
    public float loadDelay = 0.35f;
    public AudioSource successSfx;
    public GameObject successFx;

    [Tooltip("Normaler Unity UI Text.")]
    public Text feedbackText;

    [Tooltip("Während des Delays optional aktivieren.")]
    public GameObject inputBlocker;

    [Header("⚙️ Verhalten")]
    [Tooltip("Nur einmal auslösen, solange dieses Objekt lebt.")]
    public bool triggerOnce = true;

    [Tooltip("Bei triggerOnce=false nach dem Verlassen erneut aktivieren.")]
    public bool reArmOnExit = false;

    [Tooltip("Abklingzeit zwischen Auslösungen.")]
    public float retriggerCooldown = 0f;

    private bool fired;
    private float nextAllowedTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && fired)
            return;

        if (!triggerOnce && Time.time < nextAllowedTime)
            return;

        if (triggerType == TriggerType.LevelComplete &&
            achievementCoinsRequired > 0)
        {
            string coinKey = BuildCoinKey();
            int levelCoins = PlayerPrefs.GetInt(coinKey, 0);

            Debug.Log(
                $"[LevelEnd] CoinGate: {coinKey} = " +
                $"{levelCoins}/{achievementCoinsRequired}"
            );

            if (levelCoins < achievementCoinsRequired)
            {
                Debug.LogWarning(
                    $"[LevelEnd] Nicht genug Special Coins. " +
                    $"{levelCoins}/{achievementCoinsRequired}"
                );

                if (feedbackText != null)
                {
                    feedbackText.text =
                        $"❌ {achievementCoinsRequired} Special Coins benötigt!";
                }

                return;
            }
        }

        fired = true;

        if (!triggerOnce && retriggerCooldown > 0f)
        {
            nextAllowedTime = Time.time + retriggerCooldown;
        }

        StartCoroutine(FinishRoutine());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!triggerOnce && reArmOnExit)
        {
            fired = false;
        }
    }

    private IEnumerator FinishRoutine()
    {
        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                SaveLevelCompleted();
                UnlockNextLevel();
                break;

            case TriggerType.LevelUnlock:
                SaveLevelUnlock();
                break;

            case TriggerType.SecretFound:
                SaveSecretFound();
                break;
        }

        PlayerPrefs.Save();

        PlayFeedback();

        if (loadDelay > 0f)
        {
            yield return new WaitForSeconds(loadDelay);
        }

        if (triggerType == TriggerType.LevelComplete)
        {
            string sceneToLoad = returnScene;

            if (loadNextLevelOnComplete &&
                !string.IsNullOrWhiteSpace(nextLevelName))
            {
                sceneToLoad = nextLevelName;
            }

            SafeLoadScene(sceneToLoad);
        }
    }

    private void SaveLevelCompleted()
    {
        if (string.IsNullOrWhiteSpace(targetID))
        {
            Debug.LogWarning(
                "[LevelEnd] LevelComplete: targetID ist leer. " +
                "Completed-Key konnte nicht gespeichert werden."
            );

            return;
        }

        PlayerPrefs.SetInt(targetID, 1);
        PlayerPrefsKeyTracker.TrackKey(targetID);

        Debug.Log(
            $"[LevelEnd] ✅ Level abgeschlossen: " +
            $"{targetID} = 1"
        );
    }

    private void UnlockNextLevel()
    {
        if (string.IsNullOrWhiteSpace(nextLevelUnlockKey))
        {
            Debug.LogWarning(
                "[LevelEnd] ⚠️ Next Level Unlock Key ist leer. " +
                "Das nächste Level wurde NICHT freigeschaltet."
            );

            return;
        }

        PlayerPrefs.SetInt(nextLevelUnlockKey, 1);
        PlayerPrefsKeyTracker.TrackKey(nextLevelUnlockKey);

        Debug.Log(
            $"[LevelEnd] 🔓 Nächstes Level freigeschaltet: " +
            $"{nextLevelUnlockKey} = 1"
        );
    }

    private void SaveLevelUnlock()
    {
        if (string.IsNullOrWhiteSpace(targetID))
        {
            Debug.LogWarning(
                "[LevelEnd] LevelUnlock: targetID ist leer."
            );

            return;
        }

        PlayerPrefs.SetInt(targetID, 1);
        PlayerPrefsKeyTracker.TrackKey(targetID);

        Debug.Log(
            $"[LevelEnd] 🔓 Unlock gespeichert: " +
            $"{targetID} = 1"
        );
    }

    private void SaveSecretFound()
    {
        if (string.IsNullOrWhiteSpace(targetID))
        {
            Debug.LogWarning(
                "[LevelEnd] SecretFound: targetID ist leer."
            );

            return;
        }

        PlayerPrefs.SetInt(targetID, 1);
        PlayerPrefsKeyTracker.TrackKey(targetID);

        Debug.Log(
            $"[LevelEnd] 🕵️ Secret gespeichert: " +
            $"{targetID} = 1"
        );
    }

    private void PlayFeedback()
    {
        if (successFx != null)
        {
            Instantiate(
                successFx,
                transform.position,
                Quaternion.identity
            );
        }

        if (successSfx != null)
        {
            successSfx.Play();
        }

        if (feedbackText != null)
        {
            feedbackText.text = "✅ Erfolgreich!";
        }

        if (inputBlocker != null)
        {
            inputBlocker.SetActive(true);
        }
    }

    private string GetCurrentLevelName()
    {
        if (!string.IsNullOrWhiteSpace(currentLevelName))
        {
            return currentLevelName;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            return sceneName;
        }

        return targetID;
    }

    private string BuildCoinKey()
    {
        if (!string.IsNullOrWhiteSpace(coinKeyOverride))
        {
            return coinKeyOverride;
        }

        string levelName = GetCurrentLevelName();

        if (string.IsNullOrWhiteSpace(levelName))
        {
            return "UNKNOWN-special-coins";
        }

        return levelName + "-special-coins";
    }

    private void SafeLoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning(
                "[LevelEnd] Kein Szenenname angegeben."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[LevelEnd] Szene '{sceneName}' ist nicht in den Build Settings."
            );

            return;
        }

        Debug.Log($"[LevelEnd] ➡️ Lade Szene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}