using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    [Header("Klettereinstellungen")]
    public float climbSpeed = 5f; // Geschwindigkeit des Kletterns
    private Rigidbody2D playerRB; // Spieler Rigidbody
    private bool onLadder = false; // Gibt an, ob der Spieler auf der Leiter ist

    private void Start()
    {
        // Spieler finden
        playerRB = GameObject.FindWithTag("Player")?.GetComponent<Rigidbody2D>();
        if (playerRB == null)
        {
            Debug.LogError("Spieler mit Tag 'Player' nicht gefunden!");
        }
    }

    private void Update()
    {
        if (onLadder)
        {
            // Hole KeyCodes für ClimbUp und ClimbDown vom Manager
            bool upPressed = false;
            bool downPressed = false;

            // Prüfen, ob ein KeyBindManager vorhanden ist
            if (KeyBindManager.Instance != null)
            {
                upPressed = Input.GetKey(KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp"));
                downPressed = Input.GetKey(KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown"));
            }

            float verticalVelocity = 0f;
            if (upPressed && !downPressed)
            {
                // Nur hoch
                verticalVelocity = climbSpeed;
            }
            else if (downPressed && !upPressed)
            {
                // Nur runter
                verticalVelocity = -climbSpeed;
            }

            // Setze nur die vertikale Geschwindigkeit
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, verticalVelocity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Spieler betritt die Leiter
        if (collision.CompareTag("Player"))
        {
            playerRB.gravityScale = 0f;
            onLadder = true;
            Debug.Log("Spieler betritt die Leiter.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Spieler verlässt die Leiter
        if (collision.CompareTag("Player"))
        {
            playerRB.gravityScale = 2f;
            onLadder = false;
            Debug.Log("Spieler verlässt die Leiter.");
        }
    }
}
