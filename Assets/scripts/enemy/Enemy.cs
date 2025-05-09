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
    }

    void Update()
    {
        if (isDead || isTakingDamage) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        bool isNearWall = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        HandleJumpAndFallAnimation();
        Patrol();

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
            Attack(collision.gameObject);
        }
    }

    private void Attack(GameObject playerObj)
    {
        PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            animator.SetTrigger("Attack");
            playerHealth.Die();
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
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

        Debug.Log("Enemy killed!");
        AddPointsOnDeath();

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Die");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        enemyCollider.enabled = false;

        if (deathEffect != null)
        {
            deathEffect.Play();
        }

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
        Debug.Log("\u23F3 Warte auf Respawn...");
        StartCoroutine(WaitUntilPlayerIsGone());
    }

    IEnumerator WaitUntilPlayerIsGone()
    {
        while (PlayerNearby())
        {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Enemy respawning...");

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Die");
        animator.SetBool("Hit", false);
        animator.SetBool("Jump", false);
        animator.SetBool("Fall", false);
        animator.SetFloat("Speed", 0);
        animator.Play("Idle", 0, 0f);

        health = maxHealth;
        isDead = false;
        isInvulnerable = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPosition;

        spriteRenderer.enabled = true;
        enemyCollider.enabled = true;

        movingRight = lastDirectionRight;
        Flip(movingRight);

        if (TryGetComponent<EnemyShooting>(out var shooting))
            shooting.enabled = true;

        StartCoroutine(RemoveInvulnerability());

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
    }
}