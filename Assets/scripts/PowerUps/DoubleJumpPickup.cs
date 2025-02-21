using UnityEngine;

public class DoubleJumpPickup : MonoBehaviour
{
    [Tooltip("How long the player can have double jump (in seconds).")]
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Activate double jump on the player
                playerMovement.ActivateDoubleJump(duration);
            }

            Destroy(gameObject);
        }
    }
}
