using UnityEngine;
using System.Collections;

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
    public bool enableSliding = true;
    public float acceleration = 10f;
    public float deceleration = 8f;
    [Range(0f, 1f)] public float slideFactor = 0.5f;

    [Header("Ground & Wall Check")]
    public Transform[] groundCheckPoints;
    public Transform[] wallCheckPoints;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool wallJumping;
    private bool facingRight = true;
    private float coyoteTimeCounter;
    private bool canWallJump = true;

    // Double Jump
    private bool doubleJumpActive;
    private bool usedDoubleJump;

    // Movement
    private float currentSpeed = 0f;
    private KeyBindManager keyBindManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 3f;
        keyBindManager = KeyBindManager.Instance;
    }

    void Update()
    {
        HandleJumpInput();
        UpdateAnimationStates();
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
        CheckWall();
    }

    // --------------------------------
    // Movement & Controls
    // --------------------------------

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
    }

    void WallJump()
    {
        wallJumping = true;
        float wallJumpDirection = facingRight ? -1 : 1;

        // Flip the character automatically to face the other wall
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // Apply wall jump velocity
        rb.linearVelocity = new Vector2(wallJumpDirection * speed * 1.5f, jumpForce);
        anim.SetTrigger("WallJump");

        canWallJump = false;
        Invoke(nameof(EnableWallJump), 0.2f);
        Invoke(nameof(StopWallJump), 0.3f);
    }

    void EnableWallJump() => canWallJump = true;

    void StopWallJump()
    {
        wallJumping = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.8f, rb.linearVelocity.y); // Prevents abrupt stopping
    }

    // --------------------------------
    // Collision Detection
    // --------------------------------

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

    // --------------------------------
    // Animations
    // --------------------------------

    void UpdateAnimationStates()
    {
        if (isGrounded)
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
        }
        else
        {
            anim.SetBool("IsFalling", rb.linearVelocity.y < -0.1f);
            anim.SetBool("IsJumping", rb.linearVelocity.y > 0.1f && !anim.GetBool("IsFalling"));
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // --------------------------------
    // Boosts & Power-ups
    // --------------------------------

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
    }
}
