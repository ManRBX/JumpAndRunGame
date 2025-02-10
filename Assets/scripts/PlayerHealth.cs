using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    private const string LivesKey = "GlobalLives"; // Key für PlayerPrefs

    [Header("Lives Settings")]
    public int defaultLives = 3; // Standardanzahl der Leben

    private bool isInvincible = false;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;

    [Header("Damage Feedback")]
    public Color damageColor = Color.red;
    public float damageFlashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private PlayerHealthUI playerHealthUI;

    // Respawn-Position (wird durch Checkpoints aktualisiert)
    private Vector3 startPosition;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;  // Speichere die Startposition
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerHealthUI = FindObjectOfType<PlayerHealthUI>();

        // Lade die globalen Leben oder setze Standardwert
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
        // Leben aus den PlayerPrefs abrufen & reduzieren
        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) - 1;
        PlayerPrefs.SetInt(LivesKey, Mathf.Max(0, lives)); // Verhindert negative Leben
        PlayerPrefs.Save();

        // Spieler-Tode in PlayerPrefs speichern
        int deathCount = PlayerPrefs.GetInt("DeathCount", 0) + 1;
        PlayerPrefs.SetInt("DeathCount", deathCount);
        PlayerPrefs.Save();

        Debug.Log($"Spieler ist gestorben. Tode: {deathCount}, Verbleibende Leben: {lives}");

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
        PlayerPrefs.SetInt(LivesKey, defaultLives); // Leben zurücksetzen
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void AddLives(int amount)
    {
        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) + amount;
        PlayerPrefs.SetInt(LivesKey, lives);
        PlayerPrefs.Save();
        Debug.Log($"Leben hinzugefügt: {lives}");
        UpdateUI();
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Heilung: {currentHealth}");
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerHealthUI)
        {
            playerHealthUI.UpdateLebenUI();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Spieler geheilt! Aktuelles Leben: " + currentHealth);
        UpdateUI();
    }
}
