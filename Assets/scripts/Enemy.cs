using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3;

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

    [Header("Jump & Fall Animation Settings")]
    private bool isJumping = false;
    private bool isFalling = false;

    [Header("Point Settings")]
    public int pointsOnDeath = 50; // Points awarded when the enemy dies

    [Header("Respawn Settings")]
    public float respawnTime = 300f; // Respawn time in seconds (can be set in the Inspector)
    public float deathAnimationDuration = 6f; // Duration of the death animation (in seconds)

    [Header("Death Effect Settings")]
    public ParticleSystem deathEffect; // Particle system for the death effect (e.g., smoke)

    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private bool movingRight;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector3 spawnPosition;

    void Start()
    {
        Vector2 startPosition = transform.position;
        leftLimit = new Vector2(startPosition.x - leftDistance, startPosition.y);
        rightLimit = new Vector2(startPosition.x + rightDistance, startPosition.y);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        movingRight = true;
        spawnPosition = transform.position;  // Save the spawn position for respawning
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
        float moveSpeed = movingRight ? speed : -speed;
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        if (movingRight && transform.position.x >= rightLimit.x)
        {
            Flip();
        }
        else if (!movingRight && transform.position.x <= leftLimit.x)
        {
            Flip();
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

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
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
        if (isDead || isTakingDamage) return;

        health -= damage;
        Debug.Log("Enemy hit! Current HP: " + health);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageReaction());
        }
    }

    private IEnumerator DamageReaction()
    {
        isTakingDamage = true;
        animator.SetBool("Hit", true);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(damageStunTime);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            yield return null;
        }

        animator.SetBool("Hit", false);
        isTakingDamage = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy killed!");

        // Add points
        AddPointsOnDeath();

        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        // Trigger the death effect (e.g., smoke)
        if (deathEffect != null)
        {
            deathEffect.Play();
        }

        // Start respawn coroutine but don't deactivate immediately
        StartCoroutine(HandleDeathAnimation());
    }

    IEnumerator HandleDeathAnimation()
    {
        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationDuration);

        // Now deactivate and wait for respawn
        GetComponent<SpriteRenderer>().enabled = false; // Deactivate renderer
        GetComponent<Collider2D>().enabled = false;     // Disable collider
        yield return new WaitForSeconds(respawnTime);    // Wait for respawn time

        Respawn();
    }

    void Respawn()
    {
        isDead = false;
        GetComponent<SpriteRenderer>().enabled = true; // Reactivate renderer
        GetComponent<Collider2D>().enabled = true;      // Enable collider
        rb.isKinematic = false;
        transform.position = spawnPosition; // Reset to spawn position
        health = 3; // Reset health (you can modify as needed)
        animator.SetTrigger("Walk");  // Trigger walk animation after respawn
        Debug.Log("Enemy respawned!");
    }

    void AddPointsOnDeath()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddPoints(pointsOnDeath);
            Debug.Log($"+{pointsOnDeath} points received!");
        }
    }
}
