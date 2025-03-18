using UnityEngine;
using System.Collections; // Needed for Coroutines

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
    public bool enableSliding = true; // Toggle the ice effect on/off
    public float acceleration = 10f;  // Acceleration rate on ice (slower start)
    public float deceleration = 8f;   // Deceleration rate when sliding
    [Range(0f, 1f)] public float slideFactor = 0.5f;

    [Header("Ground Check Settings")]
    public Transform[] groundCheckPoints;
    private LineRenderer[] groundLineRenderers;

    [Header("Wall Check Settings")]
    public Transform[] wallCheckPoints;
    private LineRenderer[] wallLineRenderers;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool wallJumping = false;
    private bool facingRight = true;
    private float coyoteTimeCounter;
    [SerializeField] private bool canWallJump = true;

    // -----------------------------
    // Double Jump Variables
    // -----------------------------
    private bool doubleJumpActive = false;  // Is double jump currently active?
    private float doubleJumpTimer = 0f;       // How long the double jump remains active
    private bool usedDoubleJump = false;      // Has the extra jump already been used?

    // Variable for current horizontal speed
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 3f;

        InitializeGroundLineRenderers();
        InitializeWallLineRenderers();
    }

    void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateAnimationStates();

        // -----------------------------
        // Manage double jump duration
        // -----------------------------
        if (doubleJumpActive)
        {
            doubleJumpTimer -= Time.deltaTime;
            if (doubleJumpTimer <= 0f)
            {
                doubleJumpActive = false;
                usedDoubleJump = false;
            }
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
    }

    /// <summary>
    /// Moves the player based on input.
    /// If the ice effect (sliding) is enabled:
    /// - The player accelerates gradually and decelerates smoothly when the key is released.
    /// If disabled, the target speed is applied immediately.
    /// </summary>
    void HandleMovement()
    {
        bool leftPressed = false;
        bool rightPressed = false;

        if (KeyBindManager.Instance != null)
        {
            leftPressed = Input.GetKey(KeyBindManager.Instance.GetKeyCodeForAction("MoveLeft"));
            rightPressed = Input.GetKey(KeyBindManager.Instance.GetKeyCodeForAction("MoveRight"));
        }

        float move = 0f;
        if (leftPressed && !rightPressed)
            move = -1f;
        else if (rightPressed && !leftPressed)
            move = 1f;

        anim.SetBool("IsRunning", move != 0);

        float targetSpeed = move * speed;

        if (move != 0)
        {
            if (enableSliding)
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            else
                currentSpeed = targetSpeed;
        }
        else
        {
            if (enableSliding)
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            else
                currentSpeed = 0f;
        }

        if (!wallJumping)
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();
    }

    /// <summary>
    /// Processes jump input.
    /// </summary>
    void HandleJumpInput()
    {
        if (KeyBindManager.Instance != null && Input.GetKeyDown(KeyBindManager.Instance.GetKeyCodeForAction("Jump")))
        {
            // Normal jump if within coyote time and grounded
            if (coyoteTimeCounter > 0f && isGrounded)
            {
                Jump();
            }
            // Double jump if active and not yet used
            else if (doubleJumpActive && !usedDoubleJump)
            {
                Jump();
                usedDoubleJump = true;
            }
            // Wall jump if touching a wall and allowed
            else if (isTouchingWall && canWallJump)
            {
                WallJump();
            }
        }
    }

    /// <summary>
    /// Updates animations based on movement.
    /// </summary>
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

    void InitializeGroundLineRenderers()
    {
        groundLineRenderers = new LineRenderer[groundCheckPoints.Length];
        for (int i = 0; i < groundCheckPoints.Length; i++)
        {
            GameObject lineObj = new GameObject($"GroundRay_{i}");
            groundLineRenderers[i] = lineObj.AddComponent<LineRenderer>();
            groundLineRenderers[i].startWidth = 0.05f;
            groundLineRenderers[i].endWidth = 0.05f;
            groundLineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            groundLineRenderers[i].startColor = Color.red;
            groundLineRenderers[i].endColor = Color.red;
        }
    }

    void InitializeWallLineRenderers()
    {
        wallLineRenderers = new LineRenderer[wallCheckPoints.Length];
        for (int i = 0; i < wallCheckPoints.Length; i++)
        {
            GameObject lineObj = new GameObject($"WallRay_{i}");
            wallLineRenderers[i] = lineObj.AddComponent<LineRenderer>();
            wallLineRenderers[i].startWidth = 0.05f;
            wallLineRenderers[i].endWidth = 0.05f;
            wallLineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            wallLineRenderers[i].startColor = Color.red;
            wallLineRenderers[i].endColor = Color.red;
        }
    }

    /// <summary>
    /// Checks if the player is touching the ground.
    /// </summary>
    void GroundCheck()
    {
        isGrounded = false;

        foreach (Transform point in groundCheckPoints)
        {
            RaycastHit2D hit = Physics2D.Raycast(point.position, Vector2.down, groundCheckDistance, groundLayer);
            int idx = System.Array.IndexOf(groundCheckPoints, point);
            groundLineRenderers[idx].SetPosition(0, point.position);
            groundLineRenderers[idx].SetPosition(1, point.position + Vector3.down * groundCheckDistance);

            if (hit.collider != null)
            {
                isGrounded = true;
                coyoteTimeCounter = coyoteTime;
                usedDoubleJump = false;
                break;
            }
        }

        if (!isGrounded)
            coyoteTimeCounter -= Time.fixedDeltaTime;
    }

    /// <summary>
    /// Checks if the player is touching a wall.
    /// </summary>
    void WallCheck()
    {
        isTouchingWall = false;

        foreach (Transform point in wallCheckPoints)
        {
            int idx = System.Array.IndexOf(wallCheckPoints, point);
            RaycastHit2D hit = Physics2D.Raycast(point.position, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);
            wallLineRenderers[idx].SetPosition(0, point.position);
            wallLineRenderers[idx].SetPosition(1, point.position + (facingRight ? Vector3.right : Vector3.left) * wallCheckDistance);

            if (hit.collider != null)
            {
                isTouchingWall = true;
                break;
            }
        }
    }

    /// <summary>
    /// Performs a normal jump.
    /// </summary>
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("JumpTrigger");
    }

    /// <summary>
    /// Performs a wall jump.
    /// </summary>
    void WallJump()
    {
        wallJumping = true;
        float wallJumpDirection = facingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(wallJumpDirection * speed * 1.2f, jumpForce);
        anim.SetTrigger("WallJump");
        canWallJump = false;

        Invoke(nameof(EnableWallJump), 0.2f);
        Invoke(nameof(StopWallJump), 0.3f);
    }

    void EnableWallJump()
    {
        canWallJump = true;
    }

    void StopWallJump()
    {
        wallJumping = false;
    }

    /// <summary>
    /// Flips the player sprite.
    /// </summary>
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // -----------------------------------------------------------
    // SPEED BOOST COROUTINE (called e.g. from SpeedBoostPickup.cs)
    // -----------------------------------------------------------
    public IEnumerator ApplySpeedBoost(float multiplier, float boostDuration)
    {
        float originalSpeed = speed;
        speed *= multiplier;
        yield return new WaitForSeconds(boostDuration);
        speed = originalSpeed;
    }

    // -----------------------------------------------------------
    // DOUBLE JUMP ACTIVATION (called e.g. from DoubleJumpPickup.cs)
    // -----------------------------------------------------------
    public void ActivateDoubleJump(float duration)
    {
        doubleJumpActive = true;
        doubleJumpTimer = duration;
        usedDoubleJump = false;
    }
}
