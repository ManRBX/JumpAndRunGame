using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class MiniGameController : MonoBehaviour
{
    public static MiniGameController Instance;

    [Header("Global MiniGame Settings")]
    public float gameDuration = 10f;
    public TMP_Text timerText;
    public TMP_Text collectibleCountText;

    [Header("Current MiniGame Status")]
    public bool miniGameActive = false;
    public int requiredCollectibles = 0;

    private int collectedCount = 0;
    private Coroutine timerCoroutine;
    private const string LivesKey = "GlobalLives";
    private bool rewardGiven = false;

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
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleCountText != null) collectibleCountText.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, 3);
            PlayerPrefsKeyTracker.TrackKey(LivesKey); // Track default value
            PlayerPrefs.Save();
        }
    }

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
        rewardGiven = false;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
        if (collectibleCountText != null)
        {
            collectibleCountText.gameObject.SetActive(true);
            collectibleCountText.text = $"Collected: {collectedCount} / {requiredCollectibles}";
        }

        timerCoroutine = StartCoroutine(TimerCountdown(onComplete, onFailed));
        Debug.Log($"MiniGame started! Collect {requiredCollectibles} items in {duration} seconds.");
    }

    public void CollectiblePicked()
    {
        if (!miniGameActive) return;

        collectedCount++;
        Debug.Log($"Collectible collected! ({collectedCount} of {requiredCollectibles})");

        if (collectibleCountText != null)
        {
            collectibleCountText.text = $"Collected: {collectedCount} / {requiredCollectibles}";
        }

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
        if (!miniGameActive || rewardGiven) return;

        miniGameActive = false;
        rewardGiven = true;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectibleCountText != null) collectibleCountText.gameObject.SetActive(false);

        if (success)
        {
            Debug.Log("🎉 Congratulations! You collected all items!");

            int currentLives = PlayerPrefs.GetInt(LivesKey, 3) + 1;
            PlayerPrefs.SetInt(LivesKey, currentLives);
            PlayerPrefsKeyTracker.TrackKey(LivesKey); // ✅ Track geänderte Leben
            PlayerPrefs.Save();

            Debug.Log($"✅ New life added! Total lives: {currentLives}");

            PlayerHealthUI healthUI = FindFirstObjectByType<PlayerHealthUI>();
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
