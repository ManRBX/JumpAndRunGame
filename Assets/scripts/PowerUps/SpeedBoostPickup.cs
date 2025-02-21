using UnityEngine;
using System.Collections;

public class SpeedBoostPickup : MonoBehaviour
{
    [Tooltip("How much to multiply the player's speed.")]
    public float speedMultiplier = 2f;
    [Tooltip("How long the speed boost lasts (in seconds).")]
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerMovement component
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Apply the speed boost via coroutine in PlayerMovement
                playerMovement.StartCoroutine(playerMovement.ApplySpeedBoost(speedMultiplier, duration));
            }

            // Destroy this pickup so it can only be used once
            Destroy(gameObject);
        }
    }
}
