using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [Header("Global MiniGame Settings")]
    [Tooltip("Duration of the mini-game in seconds.")]
    public float gameDuration = 10f;

    [Tooltip("TMP Text field for displaying the timer.")]
    public TMP_Text timerText;

    [Header("Current MiniGame Status")]
    public bool miniGameActive = false;

    [Tooltip("Number of collectible items that need to be collected.")]
    public int requiredCollectibles = 0;

    private int collectedCount = 0;
    private Coroutine timerCoroutine;

    private const string LivesKey = "GlobalLives"; // Same key as in PlayerHealth
    private bool rewardGiven = false; // Prevents giving rewards multiple times

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
    /// Starts the mini-game with success and failure callbacks
    /// </summary>
    public void StartMiniGame(int requiredCollectibles, float duration, Action onComplete, Action onFailed)
    {
        if (miniGameActive)
        {
            Debug.LogWarning("MiniGame is already active!");
            return;
        }

        this.requiredCollectibles = requiredCollectibles;
        collectedCount = 0;
        gameDuration = duration;
        miniGameActive = true;
        rewardGiven = false; // **Ensure reward is reset**

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        timerCoroutine = StartCoroutine(TimerCountdown(onComplete, onFailed));
        Debug.Log($"MiniGame started! Collect {requiredCollectibles} items in {duration} seconds.");
    }

    public void CollectiblePicked()
    {
        if (!miniGameActive) return;

        collectedCount++;
        Debug.Log($"Collectible collected! ({collectedCount} of {requiredCollectibles})");

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
                timerText.text = "Time: " + Mathf.Ceil(timer);
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
        if (!miniGameActive || rewardGiven) return; // **Prevent double call!**

        miniGameActive = false;
        rewardGiven = true; // **Set to ensure reward is given only once**

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
            Debug.Log("🎉 Congratulations! You collected all items!");

            // Increase life **only once**
            int currentLives = PlayerPrefs.GetInt(LivesKey, 3) + 1;
            PlayerPrefs.SetInt(LivesKey, currentLives);
            PlayerPrefs.Save();
            Debug.Log($"✅ New life added! Total lives: {currentLives}");

            // Update UI
            PlayerHealthUI healthUI = FindObjectOfType<PlayerHealthUI>();
            if (healthUI != null)
            {
                healthUI.UpdateLivesUI();
            }
        }
        else
        {
            Debug.Log("⏳ Time's up! MiniGame not successful.");
        }

        OnMiniGameEnded?.Invoke(success);

        var remainingItems = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject item in remainingItems)
        {
            item.SetActive(false);
        }
    }
}
