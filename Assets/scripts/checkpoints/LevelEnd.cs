using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Reflection;

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
    [Tooltip("Name des aktuellen Levels für den Coin-Key. Fallback: targetID.")]
    public string currentLevelName;
    [Tooltip("Fallback/Standard Szene, z. B. 'Menu'.")]
    public string returnScene = "Menu";
    [Tooltip("Nach LevelComplete wirklich nextLevel laden (sonst returnScene).")]
    public bool loadNextLevelOnComplete = true;

    [Header("🏆 Achievement Settings")]
    public TriggerType triggerType = TriggerType.LevelComplete;
    [Tooltip("Basis-ID, z. B. 'Level01' – ohne Suffixe. Wird an den Tracker weitergegeben.")]
    public string targetID;
    [Tooltip("Minimale Special Coins für PASS (nur bei LevelComplete beachtet). 0 = kein Check.")]
    public int achievementCoinsRequired = 3;
    [Tooltip("Wenn gesetzt, überschreibt dies den automatisch gebauten Coin-Key.")]
    public string coinKeyOverride;

    [Header("🎬 Feedback (optional)")]
    public float loadDelay = 0.35f;
    public AudioSource successSfx;
    public GameObject successFx;
    public TMP_Text feedbackText; // optional „✅ …“-Einblendung

    [Header("⚙️ Verhalten")]
    [Tooltip("Nur einmal auslösen, solange dieses Objekt lebt.")]
    public bool triggerOnce = true;
    [Tooltip("Wenn false und reArmOnExit=true: Trigger wird beim Verlassen zurückgesetzt.")]
    public bool reArmOnExit = false;
    [Tooltip("Abklingzeit zwischen Auslösungen (nur wenn triggerOnce=false).")]
    public float retriggerCooldown = 0f;
    [Tooltip("Während des Delays optionalen Input-Blocker aktivieren.")]
    public GameObject inputBlocker;

    private bool _fired;
    private float _nextAllowedTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerOnce && _fired) return;
        if (!triggerOnce && Time.time < _nextAllowedTime) return;

        // 1) Coin-Check nur für LevelComplete
        if (triggerType == TriggerType.LevelComplete && achievementCoinsRequired > 0)
        {
            string coinKey = BuildCoinKey(); // -> "<Level>-special-coins"
            int levelCoins = PlayerPrefs.GetInt(coinKey, 0);
            Debug.Log($"Special Coins für {coinKey}: {levelCoins}/{achievementCoinsRequired}");

            if (levelCoins < achievementCoinsRequired)
            {
                Debug.Log($"❌ Du brauchst mindestens {achievementCoinsRequired} Special Coins, um hier weiterzukommen.");
                if (feedbackText) feedbackText.text = $"❌ {achievementCoinsRequired} Special Coins benötigt!";
                return;
            }
        }

        _fired = true;
        if (!triggerOnce && retriggerCooldown > 0f)
            _nextAllowedTime = Time.time + retriggerCooldown;

        StartCoroutine(FinishRoutine());
    }

    // Optionaler Re‑Arm, wenn du mehrfach durch denselben Trigger willst,
    // ohne Cooldown – setze reArmOnExit = true und triggerOnce = false
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!triggerOnce && reArmOnExit)
            _fired = false;
    }

    private IEnumerator FinishRoutine()
    {
        // 2) Achievements & Unlocks
        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                // 👉 an den Tracker NUR die Basis-ID geben (ohne Suffix).
                AchievementProgressTracker.Instance?.OnLevelCompleted(targetID);
                Debug.Log($"✅ LevelComplete Achievement ausgelöst für {targetID}.");

                // Level für später freischalten (optional, wenn gesetzt)
                if (!string.IsNullOrEmpty(nextLevelName))
                {
                    // ✅ Suffix hinten anhängen
                    string unlockKey = nextLevelName + "_Unlocked";
                    PlayerPrefs.SetInt(unlockKey, 1);
                    TryTrackKey(unlockKey);
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

        // 3) Feedback
        if (successFx) Instantiate(successFx, transform.position, Quaternion.identity);
        if (successSfx) successSfx.Play();
        if (feedbackText) feedbackText.text = "✅ Erfolgreich!";
        if (inputBlocker) inputBlocker.SetActive(true);

        // 4) kurze Pause
        if (loadDelay > 0f) yield return new WaitForSeconds(loadDelay);

        // 5) Szene laden
        string sceneToLoad = returnScene;
        if (triggerType == TriggerType.LevelComplete && loadNextLevelOnComplete && !string.IsNullOrEmpty(nextLevelName))
            sceneToLoad = nextLevelName; // bei LevelComplete bevorzugt nextLevel (wenn gewünscht)

        SafeLoadScene(sceneToLoad);
    }

    private string BuildCoinKey()
    {
        if (!string.IsNullOrEmpty(coinKeyOverride))
            return coinKeyOverride;

        // Fallback-Reihenfolge: currentLevelName → targetID
        string baseName = !string.IsNullOrEmpty(currentLevelName) ? currentLevelName : targetID;
        return string.IsNullOrEmpty(baseName) ? "UNKNOWN-special-coins" : (baseName + "-special-coins");
    }

    private void SafeLoadScene(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("⚠️ Kein Szenenname angegeben. Abbruch.");
            return;
        }

        Debug.Log($"➡️ Lade Szene: {name}");
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Versucht PlayerPrefsKeyTracker.TrackKey(string) via Reflection aufzurufen,
    /// ohne eine harte Abhängigkeit zu erzeugen.
    /// </summary>
    private void TryTrackKey(string key)
    {
        try
        {
            var t = System.Type.GetType("PlayerPrefsKeyTracker");
            if (t == null) return;
            var m = t.GetMethod("TrackKey", BindingFlags.Public | BindingFlags.Static);
            if (m == null) return;
            m.Invoke(null, new object[] { key });
        }
        catch { /* still safe */ }
    }
}
