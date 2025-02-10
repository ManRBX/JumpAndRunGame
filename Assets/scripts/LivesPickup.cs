using UnityEngine;
using TMPro;  // Für TextMeshPro UI

public class LivesPickup : MonoBehaviour
{
    public int livesToAdd = 1;  // Anzahl der Leben, die hinzugefügt werden
    public AudioClip pickupSound;  // Sound beim Einsammeln (optional)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.AddLives(livesToAdd);  // Leben hinzufügen

                // Soundeffekt abspielen (optional)
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Pickup zerstören
                Destroy(gameObject);
            }
        }
    }
}
