using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelEnd : MonoBehaviour
{
    public enum TriggerType
    {
        LevelComplete, // beendet Level; optional nextLevel freischalten
        LevelUnlock,   // setzt nur ein Unlock-Flag für targetID (kein Scene-Load)
        SecretFound    // setzt nur ein Secret-Flag für targetID (kein Scene-Load)
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

    [Header("🎯 Trigger Modus")]
    public TriggerType triggerType = TriggerType.LevelComplete;

    [Tooltip("ID für Flags/Keys (z. B. 'Level02'). Bei LevelUnlock/SecretFound erforderlich.")]
    public string targetID;

    [Header("🪙 Coin Gate (nur LevelComplete)")]
    [Tooltip("Minimale Special Coins zum Passieren (0 = kein Check).")]
    public int achievementCoinsRequired = 0;
    [Tooltip("Wenn gesetzt, überschreibt dies den automatisch gebauten Coin-Key.")]
    public string coinKeyOverride;

    [Header("🎬 Feedback (optional)")]
    public float loadDelay = 0.35f;
    public AudioSource successSfx;
    public GameObject successFx;
    public TMP_Text feedbackText; // optional „✅ …“-Einblendung
    [Tooltip("Während des Delays optionalen Input-Blocker aktivieren.")]
    public GameObject inputBlocker;

    [Header("⚙️ Verhalten")]
    [Tooltip("Nur einmal auslösen, solange dieses Objekt lebt.")]
    public bool triggerOnce = true;
    [Tooltip("Wenn false und reArmOnExit=true: Trigger wird beim Verlassen zurückgesetzt.")]
    public bool reArmOnExit = false;
    [Tooltip("Abklingzeit zwischen Auslösungen (nur wenn triggerOnce=false).")]
    public float retriggerCooldown = 0f;

    // --- intern ---
    private bool _fired;
    private float _nextAllowedTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerOnce && _fired) return;
        if (!triggerOnce && Time.time < _nextAllowedTime) return;

        // 1) Coin‑Gate (nur LevelComplete)
        if (triggerType == TriggerType.LevelComplete && achievementCoinsRequired > 0)
        {
            string coinKey = BuildCoinKey(); // -> "<Level>-special-coins"
            int levelCoins = PlayerPrefs.GetInt(coinKey, 0);
            Debug.Log($"[LevelEnd] CoinGate {coinKey}: {levelCoins}/{achievementCoinsRequired}");

            if (levelCoins < achievementCoinsRequired)
            {
                Debug.Log($"[LevelEnd] ❌ Mind. {achievementCoinsRequired} Special Coins nötig.");
                if (feedbackText) feedbackText.text = $"❌ {achievementCoinsRequired} Special Coins benötigt!";
                return;
            }
        }

        _fired = true;
        if (!triggerOnce && retriggerCooldown > 0f)
            _nextAllowedTime = Time.time + retriggerCooldown;

        StartCoroutine(FinishRoutine());
    }

    // Optionaler Re‑Arm
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!triggerOnce && reArmOnExit)
            _fired = false;
    }

    private IEnumerator FinishRoutine()
    {
        // 2) Flags/Progression (ohne Achievements)
        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                // Optional: nächstes Level als „Unlocked“ markieren
                if (!string.IsNullOrEmpty(nextLevelName))
                {
                    string unlockKey = nextLevelName + "_Unlocked";
                    PlayerPrefs.SetInt(unlockKey, 1);
                    PlayerPrefs.Save();
                    Debug.Log($"[LevelEnd] 🔓 Freigeschaltet: {nextLevelName}  (Key: {unlockKey})");
                }
                break;

            case TriggerType.LevelUnlock:
                if (!string.IsNullOrEmpty(targetID))
                {
                    string unlockKey = targetID + "_Unlocked";
                    PlayerPrefs.SetInt(unlockKey, 1);
                    PlayerPrefs.Save();
                    Debug.Log($"[LevelEnd] 🔓 LevelUnlock Flag gesetzt: {unlockKey}");
                }
                else
                {
                    Debug.LogWarning("[LevelEnd] LevelUnlock: targetID ist leer – kein Flag gesetzt.");
                }
                break;

            case TriggerType.SecretFound:
                if (!string.IsNullOrEmpty(targetID))
                {
                    string secretKey = "SecretFound_" + targetID;
                    PlayerPrefs.SetInt(secretKey, 1);
                    PlayerPrefs.Save();
                    Debug.Log($"[LevelEnd] 🕵️ SecretFlag gesetzt: {secretKey}");
                }
                else
                {
                    Debug.LogWarning("[LevelEnd] SecretFound: targetID ist leer – kein Flag gesetzt.");
                }
                break;
        }

        // 3) Feedback
        if (successFx) Instantiate(successFx, transform.position, Quaternion.identity);
        if (successSfx) successSfx.Play();
        if (feedbackText) feedbackText.text = "✅ Erfolgreich!";
        if (inputBlocker) inputBlocker.SetActive(true);

        // 4) kurze Pause
        if (loadDelay > 0f) yield return new WaitForSeconds(loadDelay);

        // 5) Szene laden (nur bei LevelComplete sinnvoll)
        string sceneToLoad = returnScene;
        if (triggerType == TriggerType.LevelComplete && loadNextLevelOnComplete && !string.IsNullOrEmpty(nextLevelName))
            sceneToLoad = nextLevelName;

        SafeLoadScene(sceneToLoad);
    }

    private string BuildCoinKey()
    {
        if (!string.IsNullOrEmpty(coinKeyOverride))
            return coinKeyOverride;

        string baseName = !string.IsNullOrEmpty(currentLevelName) ? currentLevelName : targetID;
        return string.IsNullOrEmpty(baseName) ? "UNKNOWN-special-coins" : (baseName + "-special-coins");
    }

    private void SafeLoadScene(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[LevelEnd] ⚠️ Kein Szenenname angegeben. Abbruch.");
            return;
        }

        Debug.Log($"[LevelEnd] ➡️ Lade Szene: {name}");
        SceneManager.LoadScene(name);
    }
}
