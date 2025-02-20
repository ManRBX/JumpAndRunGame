using UnityEngine;

public class SpecialCoin : MonoBehaviour
{
    public int coinIndex;  // Index of the coin (e.g., 0, 1, 2)
    public AudioClip collectSound;  // Sound effect when collected

    private AudioSource audioSource;
    private string coinKey;  // Key for this specific coin in PlayerPrefs
    private SpriteRenderer spriteRenderer;  // Renderer of the coin

    void Start()
    {
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        coinKey = $"{currentLevel}.Coin{coinIndex}";  // Unique key for this coin

        // Reference the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the coin has already been collected
        if (PlayerPrefs.HasKey(coinKey))
        {
            // Make the coin transparent
            SetTransparency(0f);
        }

        // Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = collectSound;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Only collect the special coin if it hasn't been collected yet
            if (!PlayerPrefs.HasKey(coinKey))
            {
                AddSpecialCoin();

                // Play sound effect
                if (collectSound != null)
                {
                    audioSource.Play();
                }

                // Save the coin in PlayerPrefs
                PlayerPrefs.SetInt(coinKey, 1);
                PlayerPrefs.Save();

                // Update the UI
                SpecialCoinUI uiManager = FindObjectOfType<SpecialCoinUI>();
                if (uiManager != null)
                {
                    uiManager.CollectCoin(coinIndex);  // Show the coin in the UI
                }

                // Make the coin transparent
                SetTransparency(0f);
            }
        }
    }

    void AddSpecialCoin()
    {
        const string globalKey = "GlobalSpecialCoins";  // Key for global special coins
        int globalSpecialCoins = PlayerPrefs.GetInt(globalKey, 0);
        globalSpecialCoins += 1;
        PlayerPrefs.SetInt(globalKey, globalSpecialCoins);

        Debug.Log($"Special coin collected! Total global special coins: {globalSpecialCoins}");
    }

    // Change the visibility of the coin
    private void SetTransparency(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}
