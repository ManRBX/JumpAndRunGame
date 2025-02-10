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

    [Header("Punkte-Einstellungen")]
    public int pointsOnDeath = 50; // Punkte, die beim Tod des Gegners vergeben werden

    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private bool movingRight;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        Vector2 startPosition = transform.position;
        leftLimit = new Vector2(startPosition.x - leftDistance, startPosition.y);
        rightLimit = new Vector2(startPosition.x + rightDistance, startPosition.y);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        movingRight = true;
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
        Debug.Log("Enemy getroffen! Aktuelle HP: " + health);

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

        Debug.Log("Enemy getötet!");

        // Punkte hinzufügen
        AddPointsOnDeath();

        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1.5f);
    }

    /// <summary>
    /// Vergibt Punkte, wenn der Gegner stirbt.
    /// </summary>
    private void AddPointsOnDeath()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddPoints(pointsOnDeath);
            Debug.Log($"+{pointsOnDeath} Punkte erhalten!");
        }
    }
}
