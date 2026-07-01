using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Steamworks;
using System.Linq;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;
    private const string LivesKey = "GlobalLives";
    private const string DeathKey = "DeathCount";
    private const string HealthKey = "CurrentHealth";

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

    [Header("Death Sound")]
    public AudioSource deathSound;

    [Header("Health UI")]
    public UnityEngine.UI.Text healthText;
    public float healthTextDisplayTime = 2f;
    private Coroutine healthTextCoroutine;

    private SpriteRenderer spriteRenderer;
    private PlayerHealthUI playerHealthUI;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

        if (healthText != null)
            healthText.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey(LivesKey)) { PlayerPrefs.SetInt(LivesKey, defaultLives); PlayerPrefsKeyTracker.TrackKey(LivesKey); }
        if (!PlayerPrefs.HasKey(DeathKey)) { PlayerPrefs.SetInt(DeathKey, 0); PlayerPrefsKeyTracker.TrackKey(DeathKey); }
        if (!PlayerPrefs.HasKey(HealthKey)) { PlayerPrefs.SetInt(HealthKey, maxHealth); PlayerPrefsKeyTracker.TrackKey(HealthKey); }

        if (ResetSession.wasReset)
        {
            PlayerPrefs.SetInt(LivesKey, defaultLives);
            PlayerPrefs.SetInt(DeathKey, 0);
            PlayerPrefs.SetInt(HealthKey, maxHealth);
            PlayerPrefsKeyTracker.TrackKey(LivesKey);
            PlayerPrefsKeyTracker.TrackKey(DeathKey);
            PlayerPrefsKeyTracker.TrackKey(HealthKey);
            ResetSession.wasReset = false;
        }

        currentHealth = PlayerPrefs.GetInt(HealthKey, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        PlayerPrefs.Save();
        UpdateUI();
    }

    private void SaveHealth()
    {
        PlayerPrefs.SetInt(HealthKey, currentHealth);
        PlayerPrefsKeyTracker.TrackKey(HealthKey);
        PlayerPrefs.Save();
    }

    private void ShowHealthText()
    {
        if (healthText == null) return;
        healthText.text = currentHealth + "/" + maxHealth;
        healthText.gameObject.SetActive(true);
        if (healthTextCoroutine != null) StopCoroutine(healthTextCoroutine);
        healthTextCoroutine = StartCoroutine(HideHealthTextAfterDelay());
    }

    private IEnumerator HideHealthTextAfterDelay()
    {
        yield return new WaitForSeconds(healthTextDisplayTime);
        if (healthText != null) healthText.gameObject.SetActive(false);
        healthTextCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && IsInvincibleTo("Enemy"))
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
    }

    public void TakeDamage(int damage) { TakeDamage(damage, ""); }

    public void TakeDamage(int damage, string sourceTag)
    {
        if (sourceTag == "KillZone") { Die(); return; }

        if (isInvincible)
        {
            if (!string.IsNullOrEmpty(sourceTag) && invincibleSafeTags != null && System.Array.IndexOf(invincibleSafeTags, sourceTag) != -1) return;
            else if (string.IsNullOrEmpty(sourceTag)) return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        SaveHealth();
        StartCoroutine(DamageFlash());
        ShowHealthText();
        UpdateUI();

        if (currentHealth <= 0) Die();
        else StartCoroutine(Invincibility());
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer) { spriteRenderer.color = damageColor; yield return new WaitForSeconds(damageFlashDuration); spriteRenderer.color = Color.white; }
    }

    public void Die()
    {
        if (healthText != null) healthText.gameObject.SetActive(false);
        if (deathSound != null) deathSound.Play();

        int lives = PlayerPrefs.GetInt(LivesKey, defaultLives) - 1;
        PlayerPrefs.SetInt(LivesKey, Mathf.Max(0, lives));
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        int deathCount = PlayerPrefs.GetInt(DeathKey, 0) + 1;
        PlayerPrefs.SetInt(DeathKey, deathCount);
        PlayerPrefsKeyTracker.TrackKey(DeathKey);
        PlayerPrefs.Save();

        if (lives > 0) Respawn(); else ShowGameOver();
        if (playerHealthUI != null) playerHealthUI.UpdateLivesUI();
        var saver = FindFirstObjectByType<PlayerPrefsSaver>();
        if (saver != null) saver.SavePrefsToJson();
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null) { gameOverPanel.SetActive(true); AddLife(5); }
        Time.timeScale = 0f;
    }

    void Respawn()
    {
        Vector3 checkpointPosition = CheckpointManager.instance != null ? CheckpointManager.instance.GetCheckpointPosition() : startPosition;
        checkpointPosition.y += 1f;
        transform.position = checkpointPosition;
        currentHealth = maxHealth;
        SaveHealth();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        StartCoroutine(Invincibility());
        UpdateUI();
    }

    void RestartGame()
    {
        PlayerPrefs.SetInt(LivesKey, defaultLives);
        PlayerPrefs.SetInt(HealthKey, maxHealth);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefsKeyTracker.TrackKey(HealthKey);
        PlayerPrefs.Save();
        ResetSession.wasReset = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Invincibility() { isInvincible = true; yield return new WaitForSeconds(invincibilityDuration); isInvincible = false; }
    public IEnumerator ApplyInvincibility(float d) { isInvincible = true; yield return new WaitForSeconds(d); isInvincible = false; }

    public void AddLife(int amount)
    {
        int currentLives = PlayerPrefs.GetInt(LivesKey, 3) + amount;
        PlayerPrefs.SetInt(LivesKey, currentLives);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        UpdateUI();
    }

    public void RespawnExtern() { Respawn(); }
    public void AddHealth(int amount) { currentHealth = Mathf.Min(currentHealth + amount, maxHealth); SaveHealth(); UpdateUI(); }
    void UpdateUI() { if (playerHealthUI) playerHealthUI.UpdateLivesUI(); }
    public void Heal(int amount) { currentHealth = Mathf.Min(currentHealth + amount, maxHealth); SaveHealth(); UpdateUI(); }
    public bool IsInvincibleTo(string sourceTag) { return isInvincible && invincibleSafeTags.Contains(sourceTag); }

    public void ContinueAfterGameOver()
    {
        PlayerPrefs.SetInt(LivesKey, defaultLives);
        PlayerPrefs.SetInt(HealthKey, maxHealth);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefsKeyTracker.TrackKey(HealthKey);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene("Menu"); }
}