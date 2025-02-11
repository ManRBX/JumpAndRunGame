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
    private bool rewardGiven = false; // Prevents multiple rewards

    private void Start()
    {
        // Ensure that collectible items are initially disabled
        foreach (var item in collectibleItems)
        {
            if (item != null)
                item.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated)
            return;

        if (other.CompareTag("Player"))
        {
            ActivateStarter();
        }
    }

    private void ActivateStarter()
    {
        isActivated = true;
        Debug.Log("Starter activated – MiniGame is starting!");

        if (MiniGameManager.Instance != null)
        {
            // Start the MiniGame and set success & failure callbacks
            MiniGameManager.Instance.StartMiniGame(numberOfCollectibles, miniGameDuration, OnMiniGameComplete, OnMiniGameFailed);
        }

        // Make all collectible items visible
        foreach (var item in collectibleItems)
        {
            if (item != null)
                item.SetActive(true);
        }

        // Disable the starter itself
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the player successfully collects all collectibles within the time limit.
    /// </summary>
    private void OnMiniGameComplete()
    {
        if (rewardGiven) return;
        rewardGiven = true;

        Debug.Log("✅ MiniGame successfully completed! +1 Life awarded!");

        // Increase player lives by 1
        int currentLives = PlayerPrefs.GetInt("GlobalLives", 3);
        currentLives++;
        PlayerPrefs.SetInt("GlobalLives", currentLives);
        PlayerPrefs.Save();

        // Update UI if available
        PlayerHealthUI healthUI = FindObjectOfType<PlayerHealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateLivesUI();
        }
    }

    /// <summary>
    /// Called when the MiniGame fails (time runs out).
    /// </summary>
    private void OnMiniGameFailed()
    {
        Debug.Log("❌ MiniGame failed – Collectibles are being converted into coins!");

        // Convert all remaining collectible items into coins
        foreach (var item in collectibleItems)
        {
            if (item != null && item.activeSelf) // Convert only active items
            {
                Vector3 spawnPosition = item.transform.position;
                Destroy(item); // Remove the collectible item
                Instantiate(coinPrefab, spawnPosition, Quaternion.identity); // Replace it with a coin
            }
        }
    }
}
