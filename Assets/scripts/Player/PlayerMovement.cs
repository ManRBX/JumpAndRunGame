using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FallBrakeLine
{
    public Transform lineTransform;
    [HideInInspector] public bool brakeActive = true;
    [HideInInspector] public bool wasAboveLastFrame = true;
}

[System.Serializable]
public class SlidingZone
{
    public Collider2D triggerCollider;
    public bool enableSliding = true;
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float groundCheckDistance = 0.2f;
    public float wallCheckDistance = 0.5f;
    public float coyoteTime = 0.2f;

    [Header("Slide Settings")]
    public bool enableSliding = false; // 🔥 START AUS
    public float acceleration = 10f;
    public float deceleration = 8f;
    [Range(0f, 1f)] public float slideFactor = 0.5f;

    [Header("Sliding Zonen (Inspector)")]
    public List<SlidingZone> slidingZones = new List<SlidingZone>();

    [Header("Ground & Wall Check")]
    public Transform[] groundCheckPoints;
    public Transform[] wallCheckPoints;

    [Header("Fall-Linien Bremse")]
    public List<FallBrakeLine> fallBrakeLines = new List<FallBrakeLine>();
    public float zoneMaxFallSpeed = -10f;
    public float zoneAirDrag = 3f;

    [Header("🔊 Sounds")]
    public AudioSource walkingSound;
    public AudioSource jumpSound;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool wallJumping;
    private bool facingRight = true;
    private float coyoteTimeCounter;
    private bool canWallJump = true;

    private bool doubleJumpActive;
    private bool usedDoubleJump;
    private float doubleJumpDuration;

    private float currentSpeed = 0f;
    private KeyBindManager keyBindManager;
    private MovingPlatform currentPlatform;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 3f;
        keyBindManager = KeyBindManager.Instance;
    }

    private void Update()
    {
        HandleJumpInput();
        UpdateAnimationStates();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
        CheckWall();

        if (currentPlatform != null)
        {
            rb.position += new Vector2(currentPlatform.platformVelocity.x, 0f) * Time.fixedDeltaTime;
        }

        bool activeBrake = ShouldApplyBrake();

        if (activeBrake && rb.linearVelocity.y < zoneMaxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, zoneMaxFallSpeed);
        }

        rb.linearDamping = (isGrounded || !activeBrake) ? 0f : zoneAirDrag;
    }

    void HandleMovement()
    {
        float move = GetInputMovement();
        anim.SetBool("IsRunning", move != 0);

        float targetSpeed = move * speed;

        if (move != 0)
        {
            currentSpeed = enableSliding && !wallJumping
                ? Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime)
                : targetSpeed;
        }
        else
        {
            currentSpeed = enableSliding && !wallJumping
                ? Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime)
                : 0f;
        }

        if (!wallJumping)
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        bool isWalking = Mathf.Abs(move) > 0f && isGrounded;

        if (walkingSound != null)
        {
            if (isWalking && !walkingSound.isPlaying)
                walkingSound.Play();
            else if (!isWalking && walkingSound.isPlaying)
                walkingSound.Stop();
        }
    }

    float GetInputMovement()
    {
        if (keyBindManager == null) return 0f;

        bool leftPressed = Input.GetKey(keyBindManager.GetKeyCodeForAction("MoveLeft"));
        bool rightPressed = Input.GetKey(keyBindManager.GetKeyCodeForAction("MoveRight"));

        if (leftPressed && !rightPressed) return -1f;
        if (rightPressed && !leftPressed) return 1f;
        return 0f;
    }

    void HandleJumpInput()
    {
        if (keyBindManager == null || !Input.GetKeyDown(keyBindManager.GetKeyCodeForAction("Jump"))) return;

        if (coyoteTimeCounter > 0f && isGrounded)
        {
            Jump();
        }
        else if (doubleJumpActive && !usedDoubleJump)
        {
            Jump();
            usedDoubleJump = true;
        }
        else if (isTouchingWall && canWallJump)
        {
            WallJump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("JumpTrigger");

        if (jumpSound != null)
            jumpSound.Play();
    }

    void WallJump()
    {
        wallJumping = true;
        float wallJumpDir = facingRight ? -1 : 1;

        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        rb.linearVelocity = new Vector2(wallJumpDir * speed * 1.5f, jumpForce);
        anim.SetTrigger("WallJump");

        if (jumpSound != null)
            jumpSound.Play();

        canWallJump = false;
        Invoke(nameof(EnableWallJump), 0.2f);
        Invoke(nameof(StopWallJump), 0.3f);
    }

    void EnableWallJump() => canWallJump = true;

    void StopWallJump()
    {
        wallJumping = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.8f, rb.linearVelocity.y);
    }

    void CheckGround()
    {
        isGrounded = false;

        foreach (Transform point in groundCheckPoints)
        {
            if (Physics2D.Raycast(point.position, Vector2.down, groundCheckDistance, groundLayer))
            {
                isGrounded = true;
                coyoteTimeCounter = coyoteTime;
                usedDoubleJump = false;
                return;
            }
        }

        coyoteTimeCounter -= Time.fixedDeltaTime;
    }

    void CheckWall()
    {
        isTouchingWall = false;

        foreach (Transform point in wallCheckPoints)
        {
            if (Physics2D.Raycast(point.position, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer))
            {
                isTouchingWall = true;
                return;
            }
        }
    }

    void UpdateAnimationStates()
    {
        if (isGrounded)
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
        }
        else
        {
            anim.SetBool("IsJumping", true);
            anim.SetBool("IsFalling", false);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // 🔥 FINAL: Sliding wird EINMAL gesetzt und bleibt
    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var zone in slidingZones)
        {
            if (zone.triggerCollider == other)
            {
                enableSliding = zone.enableSliding;
                Debug.Log("Sliding jetzt: " + (enableSliding ? "AN" : "AUS"));
            }
        }
    }

    public IEnumerator ApplySpeedBoost(float multiplier, float boostDuration)
    {
        float originalSpeed = speed;
        speed *= multiplier;
        yield return new WaitForSeconds(boostDuration);
        speed = originalSpeed;
    }

    public void ActivateDoubleJump(float duration)
    {
        doubleJumpActive = true;
        usedDoubleJump = false;
        doubleJumpDuration = duration;
        StartCoroutine(DoubleJumpTimer());
    }

    private IEnumerator DoubleJumpTimer()
    {
        yield return new WaitForSeconds(doubleJumpDuration);
        doubleJumpActive = false;
    }

    bool ShouldApplyBrake()
    {
        float currentY = transform.position.y;
        bool applyBrake = false;

        foreach (var line in fallBrakeLines)
        {
            if (line.lineTransform == null) continue;

            float lineY = line.lineTransform.position.y;
            bool nowBelow = currentY <= lineY;
            bool justCrossedFromAbove = line.wasAboveLastFrame && nowBelow;

            if (justCrossedFromAbove)
            {
                line.brakeActive = !line.brakeActive;

                if (line.brakeActive)
                    applyBrake = true;
            }

            line.wasAboveLastFrame = currentY > lineY;
        }

        return applyBrake;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.collider.GetComponent<MovingPlatform>();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }
}