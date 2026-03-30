using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 5f;

    [Header("Exit Fix Settings")]
    [Tooltip("Setzt beim Verlassen die vertikale Velocity auf 0, damit kein 'Boost' bleibt.")]
    public bool resetVerticalVelocityOnExit = true;

    [Tooltip("Wie lange nach dem Verlassen wird vertikale Velocity auf 0 gehalten (gegen One-Frame Boost).")]
    public float exitStabilizeTime = 0.08f;

    private Rigidbody2D playerRB;
    private bool onLadder = false;

    private float originalGravityScale = 1f;
    private float exitStabilizeUntil = 0f;

    private void Start()
    {
        playerRB = GameObject.FindWithTag("Player")?.GetComponent<Rigidbody2D>();
        if (playerRB == null)
        {
            Debug.LogError("Player with tag 'Player' not found!");
            return;
        }

        originalGravityScale = playerRB.gravityScale;
    }

    private void Update()
    {
        if (playerRB == null) return;

        // Wenn wir gerade erst runter sind: kurz stabilisieren
        if (!onLadder && Time.time < exitStabilizeUntil)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 0f);
            return;
        }

        if (onLadder)
        {
            bool upPressed = false;
            bool downPressed = false;

            if (KeyBindManager.Instance != null)
            {
                var upKey = KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp");
                var downKey = KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown");

                if (upKey != KeyCode.None) upPressed = Input.GetKey(upKey);
                if (downKey != KeyCode.None) downPressed = Input.GetKey(downKey);
            }

            float verticalVelocity = 0f;

            if (upPressed && !downPressed) verticalVelocity = climbSpeed;
            else if (downPressed && !upPressed) verticalVelocity = -climbSpeed;

            // Nur Y steuern, X bleibt wie er ist
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, verticalVelocity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerRB == null) return;

        if (collision.CompareTag("Player"))
        {
            // Gravity merken und aus
            originalGravityScale = playerRB.gravityScale;
            playerRB.gravityScale = 0f;

            // WICHTIG: vertikale Velocity killen, damit kein Rest-Boost bleibt
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 0f);

            onLadder = true;
            Debug.Log("Player entered the ladder.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerRB == null) return;

        if (collision.CompareTag("Player"))
        {
            onLadder = false;

            // Gravity wieder so wie vorher (nicht fix 2f)
            playerRB.gravityScale = originalGravityScale;

            // Rest-Y Velocity rausnehmen, damit kein hoher Sprung passiert
            if (resetVerticalVelocityOnExit)
            {
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 0f);
                exitStabilizeUntil = Time.time + Mathf.Max(0f, exitStabilizeTime);
            }

            Debug.Log("Player exited the ladder.");
        }
    }
}
