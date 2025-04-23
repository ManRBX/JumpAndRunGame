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
    // Tags, bei denen w�hrend der Invincibility kein Schaden ausgel�st werden soll.
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
        startPosition = transform.position;  // Speichere die Startposition
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

        // Globales Leben laden oder den Standardwert setzen
        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, defaultLives);
            PlayerPrefsKeyTracker.TrackKey(LivesKey);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    // �berladung f�r R�ckw�rtskompatibilit�t: Wird aufgerufen, wenn kein Tag �bergeben wird.
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, "");
    }

    /// <summary>
    /// Wendet Schaden am Spieler an.
    /// Wenn der Schaden von der "KillZone" kommt, stirbt der Spieler unabh�ngig von der Invincibility.
    /// Ist der Spieler invincible und stammt der Schaden von einem sicheren Tag (Enemy, Spike, Enemy_Projectil), wird er ignoriert.
    /// </summary>
    public void TakeDamage(int damage, string sourceTag)
    {
        // Bei KillZone immer sterben, egal ob invincible oder nicht.
        if (sourceTag == "KillZone")
        {
            Die();
            return;
        }

        // Wenn der Spieler invincible ist und der Schaden von einem sicheren Tag kommt, ignoriere den Schaden.
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
            // Kurze Invincibility nach Schaden, au�er bei KillZone-Schaden.
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
        // Leben aus PlayerPrefs holen und um 1 verringern
        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) - 1;
        PlayerPrefs.SetInt(LivesKey, Mathf.Max(0, lives)); // Sicherstellen, dass die Leben nicht negativ werden
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save(); // Speichern der neuen Leben

        Debug.Log($"Spieler gestorben. Verbleibende Leben: {lives}");

        if (lives > 0)
        {
            Respawn();
        }
        else
        {
            RestartGame();
        }

        // UI aktualisieren
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
        PlayerPrefs.SetInt(LivesKey, defaultLives); // Leben zur�cksetzen
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Kurze Invincibility, die nach erlittenem Schaden angewendet wird.
    /// </summary>
    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    /// <summary>
    /// Wendet eine l�ngere Invincibility an (z. B. durch einen Power-Up).
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
