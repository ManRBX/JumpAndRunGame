using UnityEngine;

public class Starter : MonoBehaviour
{
    [Header("Starter Settings")]
    [Tooltip("Number of collectible items that need to be collected in this MiniGame.")]
    public int numberOfCollectibles = 3;

    [Tooltip("MiniGame duration in seconds.")]
    public float miniGameDuration = 10f;

    [Tooltip("Collectible objects that become visible when the starter is activated.")]
    public GameObject[] collectibleItems;

    [Tooltip("Prefab for the coins that the collectibles should transform into.")]
    public GameObject coinPrefab;

    private bool isActivated = false;
    private bool rewardGiven = false;

    private void Start()
    {
        foreach (var item in collectibleItems)
        {
            if (item != null)
                item.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            ActivateStarter();
        }
    }

    private void ActivateStarter()
    {
        isActivated = true;
        Debug.Log("Starter activated – MiniGame is starting!");

        if (MiniGameController.Instance != null)
        {
            MiniGameController.Instance.StartMiniGame(
                numberOfCollectibles,
                miniGameDuration,
                OnMiniGameComplete,
                OnMiniGameFailed);
        }

        foreach (var item in collectibleItems)
        {
            if (item != null)
                item.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    private void OnMiniGameComplete()
    {
        if (rewardGiven) return;
        rewardGiven = true;

        Debug.Log("✅ MiniGame successfully completed! +1 Life awarded!");

        int currentLives = PlayerPrefs.GetInt("GlobalLives", 3);
        currentLives++;
        PlayerPrefs.SetInt("GlobalLives", currentLives);
        PlayerPrefsKeyTracker.TrackKey("GlobalLives"); // ✅ Track für JSON
        PlayerPrefs.Save();

        PlayerHealthUI healthUI = FindFirstObjectByType<PlayerHealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateLivesUI();
        }
    }

    private void OnMiniGameFailed()
    {
        Debug.Log("❌ MiniGame failed – Collectibles are being converted into coins!");

        foreach (var item in collectibleItems)
        {
            if (item != null && item.activeSelf)
            {
                Vector3 spawnPosition = item.transform.position;
                Destroy(item);
                Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
