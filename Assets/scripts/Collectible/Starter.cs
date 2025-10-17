using UnityEngine;
using TMPro;
using System.Collections;

public class Starter : MonoBehaviour
{
    [Header("🎮 MiniGame Einstellungen")]
    public int numberOfCollectibles = 10;
    public float miniGameDuration = 30f;
    public GameObject[] collectibleItems;
    public GameObject coinPrefab;

    [Header("🧾 UI Elemente")]
    public TMP_Text progressText;
    public TMP_Text resultText;
    public float resultHideAfterSeconds = 4f;

    [Header("💬 Texte")]
    [TextArea] public string successMessage = "🎉 Du hast alle gesammelt! Belohnung: +1 Leben";
    [TextArea] public string failMessage = "❌ Du hast nur {collected}/{total} gesammelt!";
    [TextArea] public string successEndMessage = "✅ MiniGame geschafft!";
    [TextArea] public string failEndMessage = "❌ MiniGame nicht geschafft!";

    [Header("⏳ Verhalten nach Ende")]
    public float invisibleDurationAfterEnd = 30f;

    private bool isActivated = false;
    private bool rewardGiven = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D starterCollider;

    private bool isQuitting = false; // 🧠 merkt sich, ob Szene/Spiel beendet wird

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        starterCollider = GetComponent<Collider2D>();

        foreach (var item in collectibleItems)
            if (item != null) item.SetActive(false);

        if (progressText) progressText.gameObject.SetActive(false);
        if (resultText) resultText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isActivated && MiniGameController.Instance != null && MiniGameController.Instance.miniGameActive)
        {
            int count = MiniGameController.Instance.CollectedCount;
            if (progressText != null)
                progressText.text = $"{count}/{numberOfCollectibles}";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated || !other.CompareTag("Player")) return;
        ActivateStarter();
    }

    private void ActivateStarter()
    {
        if (this == null || isQuitting) return; // 🚫 wenn schon zerstört

        isActivated = true;
        rewardGiven = false;

        Debug.Log("🎯 Starter aktiviert – MiniGame beginnt!");

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (starterCollider != null) starterCollider.enabled = false;

        if (progressText)
        {
            progressText.text = $"0/{numberOfCollectibles}";
            progressText.gameObject.SetActive(true);
        }

        if (resultText)
            resultText.gameObject.SetActive(false);

        if (MiniGameController.Instance != null)
        {
            MiniGameController.Instance.StartMiniGame(
                numberOfCollectibles,
                miniGameDuration,
                SafeCall(OnMiniGameComplete),
                SafeCall(OnMiniGameFailed)
            );
        }

        foreach (var item in collectibleItems)
            if (item != null) item.SetActive(true);
    }

    // 🧠 „Safe Wrapper“ für Actions, verhindert Fehler falls Starter zerstört wird
    private System.Action SafeCall(System.Action original)
    {
        return () =>
        {
            if (this == null || isQuitting) return;
            original?.Invoke();
        };
    }

    private void OnMiniGameComplete()
    {
        if (this == null || isQuitting) return;
        if (rewardGiven) return;
        rewardGiven = true;

        Debug.Log("✅ MiniGame abgeschlossen! +1 Leben vergeben!");

        int currentLives = PlayerPrefs.GetInt("GlobalLives", 3);
        currentLives++;
        PlayerPrefs.SetInt("GlobalLives", currentLives);
        PlayerPrefsKeyTracker.TrackKey("GlobalLives");
        PlayerPrefs.Save();

        if (resultText != null)
        {
            string finalMsg = $"{successMessage}\n<size=80%>{successEndMessage}</size>";
            resultText.text = finalMsg;
            resultText.gameObject.SetActive(true);

            TryStartCoroutine(HideAfter(resultText, resultHideAfterSeconds));
        }

        if (progressText)
            progressText.text = $"{numberOfCollectibles}/{numberOfCollectibles}";

        var healthUI = FindFirstObjectByType<PlayerHealthUI>();
        if (healthUI != null)
            healthUI.UpdateLivesUI();

        TryStartCoroutine(ReenableStarterAfterDelay());
    }

    private void OnMiniGameFailed()
    {
        if (this == null || isQuitting) return;
        if (rewardGiven) return;
        rewardGiven = true;

        Debug.Log("❌ MiniGame fehlgeschlagen – Items werden zu Coins!");

        int collected = MiniGameController.Instance != null ? MiniGameController.Instance.CollectedCount : 0;

        if (resultText != null)
        {
            string msg = failMessage
                .Replace("{collected}", collected.ToString())
                .Replace("{total}", numberOfCollectibles.ToString());
            string finalMsg = $"{msg}\n<size=80%>{failEndMessage}</size>";

            resultText.text = finalMsg;
            resultText.gameObject.SetActive(true);

            TryStartCoroutine(HideAfter(resultText, resultHideAfterSeconds));
        }

        foreach (var item in collectibleItems)
        {
            if (item != null && item.activeSelf)
            {
                Vector3 pos = item.transform.position;
                item.SetActive(false);

                if (coinPrefab != null)
                    Instantiate(coinPrefab, pos, Quaternion.identity);
            }
        }

        TryStartCoroutine(ReenableStarterAfterDelay());
    }

    // ✅ sichere Coroutine-Startfunktion
    private void TryStartCoroutine(IEnumerator routine)
    {
        if (this != null && !isQuitting && gameObject.activeInHierarchy)
        {
            try
            {
                StartCoroutine(routine);
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("⚠️ Coroutine konnte nicht gestartet werden (Starter zerstört).");
            }
        }
    }

    private IEnumerator HideAfter(TMP_Text text, float seconds)
    {
        if (text == null) yield break;
        yield return new WaitForSeconds(seconds);
        if (text != null)
            text.gameObject.SetActive(false);
    }

    private IEnumerator ReenableStarterAfterDelay()
    {
        yield return new WaitForSeconds(invisibleDurationAfterEnd);

        if (this == null || isQuitting) yield break;

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (starterCollider != null) starterCollider.enabled = true;
        isActivated = false;
        rewardGiven = false;

        Debug.Log("🔁 Starter wieder aktiviert (nicht zerstört).");
    }

    public void ShowMiniGameSuccessMessage()
    {
        if (this == null || isQuitting) return;

        if (resultText != null)
        {
            string finalMsg = $"{successMessage}\n<size=80%>{successEndMessage}</size>";
            resultText.text = finalMsg;
            resultText.gameObject.SetActive(true);
            TryStartCoroutine(HideAfter(resultText, resultHideAfterSeconds));
        }

        if (progressText != null)
            progressText.text = $"{numberOfCollectibles}/{numberOfCollectibles}";
    }
}
