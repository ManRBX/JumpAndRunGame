using UnityEngine;

public class Killzone : MonoBehaviour
{
    public int damage = 100;  // Damage inflicted by the killzone (e.g., fatal)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // Player takes full damage
            }
        }
    }
}
