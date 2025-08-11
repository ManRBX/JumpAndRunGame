using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3;
    private int maxHealth;

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

    [Header("🔊 Sounds")]
    public AudioSource stepSound;
    public AudioSource jumpSound;
    public AudioSource deathSound;

    [Header("🎯 Audio Aktivierungsdistanz")]
    public float audioPlayDistance = 10f;

    [Header("🟩 Custom Aktivierungsfeld")]
    public Vector2 customFieldSize = new Vector2(10f, 5f);
    public Vector2 customFieldOffset = Vector2.zero;

    private Transform playerTransform;
    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private bool movingRight;
    private bool initialDirectionRight;
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

        leftLimit = new Vector2(transform.position.x - leftDistance, transform.position.y);
        rightLimit = new Vector2(transform.position.x + rightDistance, transform.position.y);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        movingRight = true;
        initialDirectionRight = movingRight;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (deathSound != null) deathSound.loop = false;
        if (stepSound != null) stepSound.loop = true;
    }

    void Update()
    {
        if (isDead || isTakingDamage) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        bool isNearWall = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        HandleJumpAndFallAnimation();
        Patrol();
        HandleStepSound();

        if (isGrounded && isNearWall)
        {
            JumpOverObstacle();
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    void Patrol()
    {
        if (isDead) return;

        float moveSpeed = movingRight ? speed : -speed;
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        if (movingRight && transform.position.x >= rightLimit.x)
        {
            Flip(false);
        }
        else if (!movingRight && transform.position.x <= leftLimit.x)
        {
            Flip(true);
        }
    }

    void JumpOverObstacle()
    {
        if (!isJumping && isGrounded)
        {
            isJumping = true;
            animator.SetBool("Jump", true);
            animator.SetBool("Fall", false);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (jumpSound != null && IsPlayerInCustomField())
                jumpSound.Play();

            StartCoroutine(ResetJumpAnimation());
        }
    }

    IEnumerator ResetJumpAnimation()
    {
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        isJumping = false;
        animator.SetBool("Jump", false);
    }

    void HandleJumpAndFallAnimation()
    {
        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            isFalling = true;
            animator.SetBool("Fall", true);
        }
        else if (isGrounded)
        {
            isFalling = false;
            animator.SetBool("Fall", false);
        }
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
        if (collision.gameObject.CompareTag("Player") && canAttack)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincibleTo("Enemy"))
            {
                animator.SetTrigger("Attack");
                playerHealth.Die();
                canAttack = false;
                Invoke(nameof(ResetAttack), attackCooldown);
            }
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isTakingDamage || isInvulnerable) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log("Enemy hit! Current HP: " + health);
            StartCoroutine(DamageReaction());
        }
    }

    private IEnumerator DamageReaction()
    {
        isTakingDamage = true;
        animator.SetBool("Hit", true);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(damageStunTime);

        animator.SetBool("Hit", false);
        isTakingDamage = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetBool("Jump", false);
        animator.SetBool("Fall", false);
        animator.SetFloat("Speed", 0);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Die");

        // Sounds stoppen
        if (stepSound != null && stepSound.isPlaying)
            stepSound.Stop();

        if (jumpSound != null && jumpSound.isPlaying)
            jumpSound.Stop();

        if (deathSound != null && IsPlayerInCustomField())
            deathSound.Play();

        FindObjectOfType<AchievementProgressTracker>()?.AddKill();
        AddPointsOnDeath();
        Debug.Log("Enemy killed!");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        enemyCollider.enabled = false;

        if (deathEffect != null)
            deathEffect.Play();

        if (TryGetComponent<EnemyShooting>(out var shooting))
            shooting.enabled = false;

        lastDirectionRight = movingRight;

        StartCoroutine(HandleDeathAnimation());
    }

    IEnumerator HandleDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    void Respawn()
    {
        Debug.Log("⏳ Warte auf Respawn...");
        StartCoroutine(WaitUntilPlayerIsGone());
    }

    IEnumerator WaitUntilPlayerIsGone()
    {
        while (PlayerNearby())
        {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Enemy respawning...");

        animator.ResetTrigger("Die");
        animator.SetBool("Jump", false);
        animator.SetBool("Fall", false);
        animator.SetFloat("Speed", 0);

        health = maxHealth;
        isDead = false;

        spriteRenderer.enabled = true;
        enemyCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;

        transform.position = spawnPosition;
        movingRight = lastDirectionRight;

        Vector3 scale = transform.localScale;
        scale.x = movingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (TryGetComponent<EnemyShooting>(out var shooting))
            shooting.enabled = true;

        isInvulnerable = true;
        StartCoroutine(RemoveInvulnerability());

        Flip(true);

        Debug.Log("Enemy respawned with " + health + " HP!");
    }

    IEnumerator RemoveInvulnerability()
    {
        yield return new WaitForSeconds(0.3f);
        isInvulnerable = false;
    }

    bool PlayerNearby()
    {
        return Physics2D.OverlapCircle(spawnPosition, playerRespawnCheckRadius, playerLayer) != null;
    }

    bool IsPlayerInCustomField()
    {
        if (playerTransform == null) return false;
        Vector2 fieldCenter = (Vector2)transform.position + customFieldOffset;
        Bounds bounds = new Bounds(fieldCenter, customFieldSize);
        return bounds.Contains(playerTransform.position);
    }

    void HandleStepSound()
    {
        if (stepSound == null || !IsPlayerInCustomField())
        {
            if (stepSound != null && stepSound.isPlaying)
                stepSound.Stop();
            return;
        }

        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            if (!stepSound.isPlaying)
                stepSound.Play();
        }
        else
        {
            if (stepSound.isPlaying)
                stepSound.Stop();
        }
    }

    void AddPointsOnDeath()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddPoints(pointsOnDeath);
            Debug.Log($"+{pointsOnDeath} points received!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition == Vector3.zero ? transform.position : spawnPosition, playerRespawnCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, audioPlayDistance);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Vector3 fieldCenter = transform.position + (Vector3)customFieldOffset;
        Gizmos.DrawCube(fieldCenter, new Vector3(customFieldSize.x, customFieldSize.y, 0.1f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(fieldCenter, new Vector3(customFieldSize.x, customFieldSize.y, 0.1f));
    }

    void OnDrawGizmos()
    {
        Vector3 basePosition = Application.isPlaying ? spawnPosition : transform.position;

        Gizmos.color = Color.yellow;
        Vector3 leftPos = new Vector3(basePosition.x - leftDistance, basePosition.y, basePosition.z);
        Vector3 rightPos = new Vector3(basePosition.x + rightDistance, basePosition.y, basePosition.z);
        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawSphere(leftPos, 0.1f);
        Gizmos.DrawSphere(rightPos, 0.1f);

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 dir = Application.isPlaying ? (movingRight ? Vector3.right : Vector3.left) : Vector3.right;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
            Gizmos.DrawSphere(wallCheck.position, 0.05f);
        }
    }
}
