using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 5f;

    [Header("Exit Fix Settings")]
    [Tooltip("Setzt beim Verlassen die vertikale Geschwindigkeit auf 0.")]
    public bool resetVerticalVelocityOnExit = true;

    [Tooltip("Wie lange nach dem Verlassen die vertikale Geschwindigkeit auf 0 gehalten wird.")]
    public float exitStabilizeTime = 0.08f;

    [Header("Input Fallback")]
    [Tooltip("Wird verwendet, falls kein KeyBindManager vorhanden ist.")]
    public KeyCode fallbackClimbUpKey = KeyCode.W;

    [Tooltip("Wird verwendet, falls kein KeyBindManager vorhanden ist.")]
    public KeyCode fallbackClimbDownKey = KeyCode.S;

    private Rigidbody2D playerRB;
    private bool onLadder = false;

    private float originalGravityScale = 1f;
    private float exitStabilizeUntil = 0f;

    private void Update()
    {
        if (playerRB == null)
            return;

        if (!onLadder && Time.time < exitStabilizeUntil)
        {
            playerRB.linearVelocity = new Vector2(
                playerRB.linearVelocity.x,
                0f
            );

            return;
        }

        if (!onLadder)
            return;

        bool upPressed;
        bool downPressed;

        GetClimbInput(out upPressed, out downPressed);

        float verticalVelocity = 0f;

        if (upPressed && !downPressed)
        {
            verticalVelocity = climbSpeed;
        }
        else if (downPressed && !upPressed)
        {
            verticalVelocity = -climbSpeed;
        }

        playerRB.linearVelocity = new Vector2(
            playerRB.linearVelocity.x,
            verticalVelocity
        );
    }

    private void GetClimbInput(out bool upPressed, out bool downPressed)
    {
        upPressed = false;
        downPressed = false;

        if (KeyBindManager.Instance != null)
        {
            KeyCode upKey =
                KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp");

            KeyCode downKey =
                KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown");

            if (upKey != KeyCode.None)
            {
                upPressed = Input.GetKey(upKey);
            }
            else
            {
                upPressed = Input.GetKey(fallbackClimbUpKey);
            }

            if (downKey != KeyCode.None)
            {
                downPressed = Input.GetKey(downKey);
            }
            else
            {
                downPressed = Input.GetKey(fallbackClimbDownKey);
            }
        }
        else
        {
            upPressed = Input.GetKey(fallbackClimbUpKey);
            downPressed = Input.GetKey(fallbackClimbDownKey);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Rigidbody2D enteredPlayerRB = collision.attachedRigidbody;

        if (enteredPlayerRB == null)
        {
            enteredPlayerRB = collision.GetComponentInParent<Rigidbody2D>();
        }

        if (enteredPlayerRB == null)
        {
            Debug.LogWarning(
                "⚠️ Der Player besitzt keinen Rigidbody2D."
            );

            return;
        }

        playerRB = enteredPlayerRB;

        originalGravityScale = playerRB.gravityScale;
        playerRB.gravityScale = 0f;

        playerRB.linearVelocity = new Vector2(
            playerRB.linearVelocity.x,
            0f
        );

        exitStabilizeUntil = 0f;
        onLadder = true;

        Debug.Log(
            "🪜 Player betritt Leiter: " + playerRB.gameObject.name
        );
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (playerRB != null)
            return;

        Rigidbody2D stayingPlayerRB = collision.attachedRigidbody;

        if (stayingPlayerRB == null)
        {
            stayingPlayerRB =
                collision.GetComponentInParent<Rigidbody2D>();
        }

        if (stayingPlayerRB == null)
            return;

        playerRB = stayingPlayerRB;
        originalGravityScale = playerRB.gravityScale;
        playerRB.gravityScale = 0f;
        onLadder = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Rigidbody2D exitingPlayerRB = collision.attachedRigidbody;

        if (exitingPlayerRB == null)
        {
            exitingPlayerRB =
                collision.GetComponentInParent<Rigidbody2D>();
        }

        if (playerRB == null)
            return;

        if (exitingPlayerRB != null && exitingPlayerRB != playerRB)
            return;

        onLadder = false;

        playerRB.gravityScale = originalGravityScale;

        if (resetVerticalVelocityOnExit)
        {
            playerRB.linearVelocity = new Vector2(
                playerRB.linearVelocity.x,
                0f
            );

            exitStabilizeUntil =
                Time.time + Mathf.Max(0f, exitStabilizeTime);
        }

        Debug.Log(
            "🪜 Player verlässt Leiter: " + playerRB.gameObject.name
        );

        playerRB = null;
    }

    private void OnDisable()
    {
        RestorePlayerGravity();
    }

    private void OnDestroy()
    {
        RestorePlayerGravity();
    }

    private void RestorePlayerGravity()
    {
        if (playerRB == null)
            return;

        playerRB.gravityScale = originalGravityScale;
        playerRB = null;
        onLadder = false;
    }
}