using UnityEngine;

public class SpecialCoin : MonoBehaviour
{
    public int coinIndex;  // Index of the coin (e.g., 0, 1, 2)
    public AudioClip collectSound;  // Sound effect when collected

    private AudioSource audioSource;
    private string coinKey;  // Key for this specific coin in PlayerPrefs
    private string levelKey; // Key for storing special coins per level
    private string globalKey = "GlobalSpecialCoins"; // Key for storing total special coins globally
    private SpriteRenderer spriteRenderer;  // Renderer of the coin

    void Start()
    {
        // Get the current level name
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Create a unique key for this level's special coins (e.g., "level01-special-coins")
        coinKey = $"{currentLevel}.Coin{coinIndex}";
        levelKey = $"{currentLevel}-special-coins";  // Key for storing the number of special coins collected in this level

        // Reference the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the coin has already been collected
        if (IsCoinCollected())
        {
            // Make the coin transparent if already collected
            SetTransparency(0f);
        }

        // Add AudioSource component to play sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = collectSound;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Only collect the special coin if it hasn't been collected yet
            if (!IsCoinCollected())
            {
                AddSpecialCoin();

                // Play sound effect on collection
                if (collectSound != null)
                {
                    audioSource.Play();
                }

                // Save that the coin has been collected for this level
                PlayerPrefs.SetInt(coinKey, 1); // Mark the coin as collected
                PlayerPrefs.Save();

                // Update the UI
                SpecialCoinUI uiManager = FindObjectOfType<SpecialCoinUI>();
                if (uiManager != null)
                {
                    uiManager.UpdateCoinUI();  // Update the coin count in the UI
                }

                // Make the coin transparent (invisible)
                SetTransparency(0f);
            }
        }
    }

    void AddSpecialCoin()
    {
        // Get the current number of special coins collected for this level
        int currentCoins = GetSpecialCoins();
        currentCoins++;  // Increment by 1 for this coin

        // Save the updated coin count for this level
        PlayerPrefs.SetInt(levelKey, currentCoins);
        PlayerPrefs.Save();

        // Add to global special coins
        AddGlobalSpecialCoin();

        Debug.Log($"Special coin collected! Total special coins for this level: {currentCoins}");
    }

    // Get the total special coins collected for this level
    int GetSpecialCoins()
    {
        return PlayerPrefs.GetInt(levelKey, 0);  // If no special coins are saved, default to 0
    }

    // Add to the global special coins count
    void AddGlobalSpecialCoin()
    {
        int globalSpecialCoins = PlayerPrefs.GetInt(globalKey, 0);
        globalSpecialCoins++;  // Increment global special coins
        PlayerPrefs.SetInt(globalKey, globalSpecialCoins);
        PlayerPrefs.Save();

        Debug.Log($"Total global special coins: {globalSpecialCoins}");
    }

    // Check if the coin has been collected (if its value is saved)
    bool IsCoinCollected()
    {
        return PlayerPrefs.GetInt(coinKey, 0) > 0;
    }

    // Change the visibility of the coin
    private void SetTransparency(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;  // Set the alpha transparency
            spriteRenderer.color = color;
        }
    }
}
