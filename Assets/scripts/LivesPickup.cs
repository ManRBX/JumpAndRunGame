using UnityEngine;
using TMPro;  // For TextMeshPro UI

public class LivesPickup : MonoBehaviour
{
    public int livesToAdd = 1;  // Number of lives to add
    public AudioClip pickupSound;  // Sound played when picked up (optional)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.AddLives(livesToAdd);  // Add lives

                // Play sound effect (optional)
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Destroy the pickup object
                Destroy(gameObject);
            }
        }
    }
}
