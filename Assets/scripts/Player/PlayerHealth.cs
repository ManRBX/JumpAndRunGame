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
    public int defaultLives = 5; // Default number of lives

    private bool isInvincible = false;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;  // For damage invincibility
    public string[] invincibleSafeTags = new string[] { "Enemy", "Spike", "Enemy_Projectil" };

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
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, defaultLives);
            PlayerPrefsKeyTracker.TrackKey(LivesKey);
            PlayerPrefs.Save();
        }

        // Überprüfe Reset-Flag, um Leben zurückzusetzen
        if (ResetSession.wasReset)
        {
            PlayerPrefs.SetInt(LivesKey, defaultLives);
            PlayerPrefsKeyTracker.TrackKey(LivesKey);
            PlayerPrefs.Save();
            ResetSession.wasReset = false;
            Debug.Log("🚀 Initiale Leben nach Reset gesetzt: " + defaultLives);
        }

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, "");
    }

    public void TakeDamage(int damage, string sourceTag)
    {
        if (sourceTag == "KillZone")
        {
            Die();
            return;
        }

        if (isInvincible)
        {
            if (!string.IsNullOrEmpty(sourceTag) && invincibleSafeTags != null && System.Array.IndexOf(invincibleSafeTags, sourceTag) != -1)
            {
                return;
            }
            else if (string.IsNullOrEmpty(sourceTag))
            {
                return;
            }
        }

        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(Invincibility());
        }
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
        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) - 1;
        PlayerPrefs.SetInt(LivesKey, Mathf.Max(0, lives));
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save();

        Debug.Log($"Spieler gestorben. Verbleibende Leben: {lives}");

        if (lives > 0)
        {
            Respawn();
        }
        else
        {
            RestartGame();
        }

        if (playerHealthUI != null)
        {
            playerHealthUI.UpdateLivesUI();
        }
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
        PlayerPrefs.SetInt(LivesKey, defaultLives);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save();

        ResetSession.wasReset = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

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
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
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