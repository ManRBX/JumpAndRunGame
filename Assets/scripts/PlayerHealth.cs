using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    private const string LivesKey = "GlobalLives"; // Key for PlayerPrefs

    [Header("Lives Settings")]
    public int defaultLives = 3; // Default number of lives

    private bool isInvincible = false;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;  // For damage invincibility

    [Header("Damage Feedback")]
    public Color damageColor = Color.red;
    public float damageFlashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private PlayerHealthUI playerHealthUI;

    // Respawn position (updated by checkpoints)
    private Vector3 startPosition;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;  // Store the starting position
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerHealthUI = FindObjectOfType<PlayerHealthUI>();

        // Load global lives or set default value
        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, defaultLives);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        // Apply short invincibility after taking damage
        StartCoroutine(Invincibility());
        UpdateUI();
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(damageFlashDuration);
            spriteRenderer.color = Color.white;
        }
    }

    public void Die()
    {
        // Retrieve lives from PlayerPrefs & reduce them
        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) - 1;
        PlayerPrefs.SetInt(LivesKey, Mathf.Max(0, lives)); // Prevent negative lives
        PlayerPrefs.Save();

        // Store player deaths in PlayerPrefs
        int deathCount = PlayerPrefs.GetInt("DeathCount", 0) + 1;
        PlayerPrefs.SetInt("DeathCount", deathCount);
        PlayerPrefs.Save();

        Debug.Log($"Player died. Death count: {deathCount}, Remaining lives: {lives}");

        if (lives > 0)
        {
            Respawn();
        }
        else
        {
            RestartGame();
        }
        UpdateUI();
    }

    void Respawn()
    {
        Vector3 checkpointPosition = CheckpointManager.instance != null
            ? CheckpointManager.instance.GetCheckpointPosition()
            : startPosition;

        float respawnHeightOffset = 1f;
        checkpointPosition.y += respawnHeightOffset;

        transform.position = checkpointPosition;
        currentHealth = maxHealth;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Respawned at: " + checkpointPosition);
    }

    void RestartGame()
    {
        PlayerPrefs.SetInt(LivesKey, defaultLives); // Reset lives
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Short invincibility applied after taking damage.
    /// </summary>
    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    /// <summary>
    /// Applies invincibility for a specified duration (used by the Invincibility power-up).
    /// </summary>
    public IEnumerator ApplyInvincibility(float powerUpDuration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(powerUpDuration);
        isInvincible = false;
    }

    public void AddLives(int amount)
    {
        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) + amount;
        PlayerPrefs.SetInt(LivesKey, lives);
        PlayerPrefs.Save();
        Debug.Log($"Lives added: {lives}");
        UpdateUI();
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Healing: {currentHealth}");
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerHealthUI)
        {
            playerHealthUI.UpdateLivesUI();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player healed! Current health: " + currentHealth);
        UpdateUI();
    }
}
