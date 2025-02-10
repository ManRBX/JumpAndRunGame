using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [Header("Globale MiniGame Einstellungen")]
    [Tooltip("Dauer des Mini-Games in Sekunden.")]
    public float gameDuration = 10f;

    [Tooltip("TMP Textfeld zur Anzeige des Timers.")]
    public TMP_Text timerText;

    [Header("Aktueller MiniGame Status")]
    public bool miniGameActive = false;

    [Tooltip("Anzahl der Collectible Items, die eingesammelt werden müssen.")]
    public int requiredCollectibles = 0;

    private int collectedCount = 0;
    private Coroutine timerCoroutine;

    private const string LivesKey = "GlobalLives"; // Gleicher Key wie in PlayerHealth
    private bool rewardGiven = false; // Verhindert doppeltes Leben hinzufügen

    public event Action<bool> OnMiniGameEnded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, 3);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Startet das MiniGame mit Erfolgs- und Fehlgeschlagen-Callback
    /// </summary>
    public void StartMiniGame(int requiredCollectibles, float duration, Action onComplete, Action onFailed)
    {
        if (miniGameActive)
        {
            Debug.LogWarning("MiniGame ist bereits aktiv!");
            return;
        }

        this.requiredCollectibles = requiredCollectibles;
        collectedCount = 0;
        gameDuration = duration;
        miniGameActive = true;
        rewardGiven = false; // **Sicherstellen, dass Belohnung zurückgesetzt wird**

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        timerCoroutine = StartCoroutine(TimerCountdown(onComplete, onFailed));
        Debug.Log($"MiniGame gestartet! Sammle {requiredCollectibles} Items in {duration} Sekunden.");
    }

    public void CollectiblePicked()
    {
        if (!miniGameActive) return;

        collectedCount++;
        Debug.Log($"Collectible eingesammelt! ({collectedCount} von {requiredCollectibles})");

        if (collectedCount >= requiredCollectibles)
        {
            EndMiniGame(true);
        }
    }

    private IEnumerator TimerCountdown(Action onComplete, Action onFailed)
    {
        float timer = gameDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = "Zeit: " + Mathf.Ceil(timer);
            }
            yield return null;
        }

        if (collectedCount >= requiredCollectibles)
        {
            onComplete?.Invoke();
            EndMiniGame(true);
        }
        else
        {
            onFailed?.Invoke();
            EndMiniGame(false);
        }
    }

    public void EndMiniGame(bool success)
    {
        if (!miniGameActive || rewardGiven) return; // **Doppelten Aufruf verhindern!**

        miniGameActive = false;
        rewardGiven = true; // **Setze, damit nur einmalig Belohnung vergeben wird**

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        if (success)
        {
            Debug.Log("🎉 Glückwunsch! Du hast alle Items eingesammelt!");

            // Leben **einmalig** erhöhen
            int currentLives = PlayerPrefs.GetInt(LivesKey, 3) + 1;
            PlayerPrefs.SetInt(LivesKey, currentLives);
            PlayerPrefs.Save();
            Debug.Log($"✅ Neues Leben hinzugefügt! Gesamtleben: {currentLives}");

            // UI aktualisieren
            PlayerHealthUI healthUI = FindObjectOfType<PlayerHealthUI>();
            if (healthUI != null)
            {
                healthUI.UpdateLebenUI();
            }
        }
        else
        {
            Debug.Log("⏳ Zeit abgelaufen! MiniGame nicht erfolgreich.");
        }

        OnMiniGameEnded?.Invoke(success);

        var remainingItems = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject item in remainingItems)
        {
            item.SetActive(false);
        }
    }
}
