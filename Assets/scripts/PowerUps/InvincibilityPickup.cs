using UnityEngine;

public class InvincibilityPickup : MonoBehaviour
{
    [Tooltip("How long the player is invincible (in seconds).")]
    public float duration = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Access PlayerHealth script
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Start invincibility on the player
                playerHealth.StartCoroutine(playerHealth.ApplyInvincibility(duration));
            }

            Destroy(gameObject);
        }
    }
}
