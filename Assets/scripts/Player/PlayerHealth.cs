using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Steamworks;
using System.Linq;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    private const string LivesKey = "GlobalLives";
    private const string DeathKey = "DeathCount";

    [Header("Lives Settings")]
    public int defaultLives = 5;

    public bool IsInvincible => isInvincible;
    public string[] invincibleSafeTags = new string[] { "Enemy", "Spike", "Enemy_Projectil" };

    private bool isInvincible = false;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;

    [Header("Damage Feedback")]
    public Color damageColor = Color.red;
    public float damageFlashDuration = 0.1f;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("🔊 Death Sound")]
    public AudioSource deathSound;

    private SpriteRenderer spriteRenderer;
    private PlayerHealthUI playerHealthUI;
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
        }

        if (!PlayerPrefs.HasKey(DeathKey))
        {
            PlayerPrefs.SetInt(DeathKey, 0);
            PlayerPrefsKeyTracker.TrackKey(DeathKey);
        }

        if (ResetSession.wasReset)
        {
            PlayerPrefs.SetInt(LivesKey, defaultLives);
            PlayerPrefs.SetInt(DeathKey, 0);
            PlayerPrefsKeyTracker.TrackKey(LivesKey);
            PlayerPrefsKeyTracker.TrackKey(DeathKey);
            ResetSession.wasReset = false;
            Debug.Log("🚀 Leben & Tode nach Reset gesetzt");
        }

        PlayerPrefs.Save();
        UpdateUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && IsInvincibleTo("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
        }
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
                return;
            else if (string.IsNullOrEmpty(sourceTag))
                return;
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
        // 🔊 Death Sound abspielen
        if (deathSound != null)
            deathSound.Play();

        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) - 1;
        PlayerPrefs.SetInt(LivesKey, Mathf.Max(0, lives));
        PlayerPrefsKeyTracker.TrackKey(LivesKey);

        int deathCount = PlayerPrefs.GetInt(DeathKey, 0) + 1;
        PlayerPrefs.SetInt(DeathKey, deathCount);
        PlayerPrefsKeyTracker.TrackKey(DeathKey);

        PlayerPrefs.Save();
        Debug.Log($"☠️ Spieler gestorben. Verbleibende Leben: {lives} | Gesamt Tode: {deathCount}");

        if (lives > 0)
        {
            Respawn();
        }
        else
        {
            ShowGameOver();
        }

        if (playerHealthUI != null)
        {
            playerHealthUI.UpdateLivesUI();
        }

        var saver = FindFirstObjectByType<PlayerPrefsSaver>();
        if (saver != null)
        {
            saver.SavePrefsToJson();
            Debug.Log("📂 PlayerPrefs wurden auch als JSON gespeichert.");
        }
    }

    void ShowGameOver()
    {
        Debug.Log("💀 Game Over!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            AddLife(5); // UI aktualisieren, auch wenn keine Leben mehr übrig sind
        }

        Time.timeScale = 0f;
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

        Debug.Log("🔄 Respawned at: " + checkpointPosition);
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

    public void AddLife(int amount)
    {
        int currentLives = PlayerPrefs.GetInt(LivesKey, 3);
        currentLives += amount;
        PlayerPrefs.SetInt(LivesKey, currentLives);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        Debug.Log($"❤️ Leben: {currentLives}");
        UpdateUI();
    }

    public void RespawnExtern()
    {
        Respawn();
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"💊 Healing: {currentHealth}");
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
        Debug.Log("🩹 Player healed! Current health: " + currentHealth);
        UpdateUI();
    }

    public bool IsInvincibleTo(string sourceTag)
    {
        return isInvincible && invincibleSafeTags.Contains(sourceTag);
    }

    // 👇 Wird vom "Play"-Button im Game Over Panel aufgerufen
    public void ContinueAfterGameOver()
    {
        PlayerPrefs.SetInt(LivesKey, defaultLives);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save();

        Time.timeScale = 1f; // Spiel wieder starten
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("🔁 Neustart nach Game Over mit 5 Leben.");
    }

    // 👇 Wird vom "Main Menu"-Button im Game Over Panel aufgerufen
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        Debug.Log("🏠 Zurück ins Hauptmenü.");
    }
}
