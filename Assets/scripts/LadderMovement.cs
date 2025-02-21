using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 5f; // Speed of climbing
    private Rigidbody2D playerRB; // Player's Rigidbody
    private bool onLadder = false; // Indicates whether the player is on the ladder

    private void Start()
    {
        // Find the player
        playerRB = GameObject.FindWithTag("Player")?.GetComponent<Rigidbody2D>();
        if (playerRB == null)
        {
            Debug.LogError("Player with tag 'Player' not found!");
        }
    }

    private void Update()
    {
        if (onLadder)
        {
            // Get KeyCodes for ClimbUp and ClimbDown from the manager
            bool upPressed = false;
            bool downPressed = false;

            // Check if a KeyBindManager exists
            if (KeyBindManager.Instance != null)
            {
                upPressed = Input.GetKey(KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp"));
                downPressed = Input.GetKey(KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown"));
            }

            float verticalVelocity = 0f;
            if (upPressed && !downPressed)
            {
                // Only moving up
                verticalVelocity = climbSpeed;
            }
            else if (downPressed && !upPressed)
            {
                // Only moving down
                verticalVelocity = -climbSpeed;
            }

            // Set only the vertical velocity
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, verticalVelocity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player enters the ladder
        if (collision.CompareTag("Player"))
        {
            playerRB.gravityScale = 0f;
            onLadder = true;
            Debug.Log("Player entered the ladder.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player exits the ladder
        if (collision.CompareTag("Player"))
        {
            playerRB.gravityScale = 2f;
            onLadder = false;
            Debug.Log("Player exited the ladder.");
        }
    }
}
