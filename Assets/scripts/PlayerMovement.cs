using UnityEngine;

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
    private bool canWallJump = true;

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
    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
    }

    /// <summary>
    /// Moves the player based on input.
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
        {
            move = -1f;
        }
        else if (rightPressed && !leftPressed)
        {
            move = 1f;
        }

        anim.SetBool("IsRunning", move != 0);

        if (!wallJumping)
        {
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }

        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }
    }

    /// <summary>
    /// Handles player jump input.
    /// </summary>
    void HandleJumpInput()
    {
        if (KeyBindManager.Instance != null
            && Input.GetKeyDown(KeyBindManager.Instance.GetKeyCodeForAction("Jump")))
        {
            if (coyoteTimeCounter > 0f && isGrounded)
            {
                Jump();
            }
            else if (isTouchingWall)
            {
                WallJump();
            }
        }
    }

    /// <summary>
    /// Updates animation states based on player movement.
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

        for (int i = 0; i < groundCheckPoints.Length; i++)
        {
            Transform point = groundCheckPoints[i];
            RaycastHit2D groundHit = Physics2D.Raycast(point.position, Vector2.down, groundCheckDistance, groundLayer);

            groundLineRenderers[i].SetPosition(0, point.position);
            groundLineRenderers[i].SetPosition(1, point.position + Vector3.down * groundCheckDistance);

            if (groundHit.collider != null)
            {
                isGrounded = true;
                groundLineRenderers[i].startColor = Color.green;
                groundLineRenderers[i].endColor = Color.green;
            }
            else
            {
                groundLineRenderers[i].startColor = Color.red;
                groundLineRenderers[i].endColor = Color.red;
            }
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            canWallJump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Checks if the player is touching a wall.
    /// </summary>
    void WallCheck()
    {
        isTouchingWall = false;

        for (int i = 0; i < wallCheckPoints.Length; i++)
        {
            Transform point = wallCheckPoints[i];
            RaycastHit2D wallHit = Physics2D.Raycast(point.position, Vector2.right * (facingRight ? 1 : -1), wallCheckDistance, wallLayer);

            wallLineRenderers[i].SetPosition(0, point.position);
            wallLineRenderers[i].SetPosition(1, point.position + Vector3.right * (facingRight ? 1 : -1) * wallCheckDistance);

            if (wallHit.collider != null)
            {
                isTouchingWall = true;
                wallLineRenderers[i].startColor = Color.green;
                wallLineRenderers[i].endColor = Color.green;
            }
            else
            {
                wallLineRenderers[i].startColor = Color.red;
                wallLineRenderers[i].endColor = Color.red;
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
}
