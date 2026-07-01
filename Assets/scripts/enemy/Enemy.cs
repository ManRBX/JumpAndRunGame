using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3;
    private int maxHealth;

    [Header("Health UI")]
    public Text healthText;
    public float healthTextDisplayTime = 2f;
    private Coroutine healthTextCoroutine;

    [Header("Movement Settings")]
    public float speed = 3f;
    public float leftDistance = 5f;
    public float rightDistance = 5f;
    public float jumpForce = 7f;

    [Header("Ground & Wall Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.5f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    private bool canAttack = true;

    [Header("Damage Settings")]
    public float damageStunTime = 0.5f;
    private bool isTakingDamage = false;
    private bool isDead = false;
    private bool isInvulnerable = false;

    [Header("Jump & Fall Animation Settings")]
    private bool isJumping = false;
    private bool isFalling = false;

    [Header("Point Settings")]
    public int pointsOnDeath = 50;

    [Header("Respawn Settings")]
    public float respawnTime = 5f;
    public float deathAnimationDuration = 1.5f;

    [Header("Death Effect Settings")]
    public ParticleSystem deathEffect;

    [Header("Respawn Proximity Check")]
    public float playerRespawnCheckRadius = 3f;
    public LayerMask playerLayer;

    [Header("Sounds")]
    public AudioSource stepSound;
    public AudioSource jumpSound;
    public AudioSource deathSound;

    [Header("Audio Aktivierungsdistanz")]
    public float audioPlayDistance = 10f;

    [Header("Custom Aktivierungsfeld")]
    public Vector2 customFieldSize = new Vector2(10f, 5f);
    public Vector2 customFieldOffset = Vector2.zero;

    private Transform playerTransform;
    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private bool movingRight;
    private bool lastDirectionRight;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    private Vector3 spawnPosition;

    void Start()
    {
        maxHealth = health;
        spawnPosition = transform.position;

        if (healthText != null)
            healthText.gameObject.SetActive(false);

        leftLimit = new Vector2(transform.position.x - leftDistance, transform.position.y);
        rightLimit = new Vector2(transform.position.x + rightDistance, transform.position.y);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        movingRight = true;

        var player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player ? player.transform : null;

        if (deathSound != null) deathSound.loop = false;
        if (stepSound != null) stepSound.loop = true;
    }

    void Update()
    {
        if (isDead || isTakingDamage) return;
        if (rb == null) return;

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        bool isNearWall = false;
        if (wallCheck != null)
            isNearWall = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        HandleJumpAndFallAnimation();
        Patrol();
        HandleStepSound();

        if (isGrounded && isNearWall)
            JumpOverObstacle();

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void ShowHealthText()
    {
        if (healthText == null) return;
        healthText.text = health + "/" + maxHealth;
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

    void Patrol()
    {
        if (isDead || rb == null) return;
        float moveSpeed = movingRight ? speed : -speed;
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        if (movingRight && transform.position.x >= rightLimit.x) Flip(false);
        else if (!movingRight && transform.position.x <= leftLimit.x) Flip(true);
    }

    void JumpOverObstacle()
    {
        if (rb == null || isJumping || !isGrounded) return;
        isJumping = true;
        if (animator != null) { animator.SetBool("Jump", true); animator.SetBool("Fall", false); }
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (jumpSound != null && IsPlayerInCustomField()) jumpSound.Play();
        StartCoroutine(ResetJumpAnimation());
    }

    IEnumerator ResetJumpAnimation()
    {
        if (rb == null) yield break;
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        isJumping = false;
        if (animator != null) animator.SetBool("Jump", false);
    }

    void HandleJumpAndFallAnimation()
    {
        if (rb == null || animator == null) return;
        if (!isGrounded && rb.linearVelocity.y < -0.1f) { isFalling = true; animator.SetBool("Fall", true); }
        else if (isGrounded) { isFalling = false; animator.SetBool("Fall", false); }
    }

    void Flip(bool toRight)
    {
        movingRight = toRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (movingRight ? 1 : -1);
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player") && canAttack)
        {
            var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            bool canHit = true;
            if (playerHealth != null)
            {
                var m = playerHealth.GetType().GetMethod("IsInvincibleTo", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (m != null) { object res = m.Invoke(playerHealth, new object[] { "Enemy" }); if (res is bool b) canHit = !b; }
            }
            if (playerHealth != null && canHit)
            {
                if (animator != null) animator.SetTrigger("Attack");
                playerHealth.TakeDamage(attackDamage, "Enemy");
                canAttack = false;
                Invoke(nameof(ResetAttack), attackCooldown);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isTakingDamage || isInvulnerable) return;
        health -= damage;
        ShowHealthText();
        if (health <= 0) Die();
        else StartCoroutine(DamageReaction());
    }

    private void ResetAttack() { canAttack = true; }

    private IEnumerator DamageReaction()
    {
        isTakingDamage = true;
        if (animator != null) animator.SetBool("Hit", true);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(damageStunTime);
        if (animator != null) animator.SetBool("Hit", false);
        isTakingDamage = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if (healthText != null) healthText.gameObject.SetActive(false);
        if (animator != null) { animator.SetBool("Jump", false); animator.SetBool("Fall", false); animator.SetFloat("Speed", 0); animator.ResetTrigger("Attack"); animator.SetTrigger("Die"); }
        if (stepSound != null && stepSound.isPlaying) stepSound.Stop();
        if (jumpSound != null && jumpSound.isPlaying) jumpSound.Stop();
        if (deathSound != null && IsPlayerInCustomField()) deathSound.Play();
        AddPointsOnDeath();
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.bodyType = RigidbodyType2D.Kinematic; }
        if (enemyCollider != null) enemyCollider.enabled = false;
        if (deathEffect != null) deathEffect.Play();
        if (TryGetComponent<EnemyShooting>(out var shooting)) shooting.enabled = false;
        lastDirectionRight = movingRight;
        StartCoroutine(HandleDeathAnimation());
    }

    IEnumerator HandleDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    void Respawn() { StartCoroutine(WaitUntilPlayerIsGone()); }

    IEnumerator WaitUntilPlayerIsGone()
    {
        while (PlayerNearby()) yield return new WaitForSeconds(0.2f);
        if (animator != null) { animator.ResetTrigger("Die"); animator.SetBool("Jump", false); animator.SetBool("Fall", false); animator.SetFloat("Speed", 0); }
        health = maxHealth;
        isDead = false;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (enemyCollider != null) enemyCollider.enabled = true;
        if (rb != null) { rb.bodyType = RigidbodyType2D.Dynamic; rb.linearVelocity = Vector2.zero; }
        transform.position = spawnPosition;
        movingRight = lastDirectionRight;
        Vector3 scale = transform.localScale;
        scale.x = movingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
        if (TryGetComponent<EnemyShooting>(out var shooting)) shooting.enabled = true;
        isInvulnerable = true;
        StartCoroutine(RemoveInvulnerability());
    }

    IEnumerator RemoveInvulnerability() { yield return new WaitForSeconds(0.3f); isInvulnerable = false; }
    bool PlayerNearby() { return Physics2D.OverlapCircle(spawnPosition, playerRespawnCheckRadius, playerLayer) != null; }

    bool IsPlayerInCustomField()
    {
        if (playerTransform == null) return false;
        Vector3 fieldCenter = (Vector2)transform.position + customFieldOffset;
        Bounds bounds = new Bounds(fieldCenter, new Vector3(customFieldSize.x, customFieldSize.y, 0.1f));
        return bounds.Contains(playerTransform.position);
    }

    void HandleStepSound()
    {
        if (stepSound == null || !IsPlayerInCustomField()) { if (stepSound != null && stepSound.isPlaying) stepSound.Stop(); return; }
        if (isGrounded && rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f) { if (!stepSound.isPlaying) stepSound.Play(); }
        else { if (stepSound.isPlaying) stepSound.Stop(); }
    }

    void AddPointsOnDeath() { if (CoinManager.Instance != null) CoinManager.Instance.AddPoints(pointsOnDeath); }

    void OnDrawGizmosSelected()
    {
        Vector3 basePos = (Application.isPlaying && spawnPosition != Vector3.zero) ? spawnPosition : transform.position;
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(basePos, playerRespawnCheckRadius);
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, audioPlayDistance);
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Vector3 fieldCenter = transform.position + (Vector3)customFieldOffset;
        Vector3 size3 = new Vector3(customFieldSize.x, customFieldSize.y, 0.1f);
        Gizmos.DrawCube(fieldCenter, size3); Gizmos.color = Color.green; Gizmos.DrawWireCube(fieldCenter, size3);
    }

    void OnDrawGizmos()
    {
        Vector3 basePosition = Application.isPlaying && spawnPosition != Vector3.zero ? spawnPosition : transform.position;
        Gizmos.color = Color.yellow;
        Vector3 leftPos = new Vector3(basePosition.x - leftDistance, basePosition.y, basePosition.z);
        Vector3 rightPos = new Vector3(basePosition.x + rightDistance, basePosition.y, basePosition.z);
        Gizmos.DrawLine(leftPos, rightPos); Gizmos.DrawSphere(leftPos, 0.1f); Gizmos.DrawSphere(rightPos, 0.1f);
        if (groundCheck != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, 0.2f); }
        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 dir = Application.isPlaying ? (movingRight ? Vector3.right : Vector3.left) : Vector3.right;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
            Gizmos.DrawSphere(wallCheck.position, 0.05f);
        }
    }
}