using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifeTime = 2f;  // How long the bullet exists
    public int damage = 1;       // Damage dealt by the bullet

    [Header("🔊 Schuss-Sound")]
    public AudioSource shootSound;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
        }

        // Schuss-Sound abspielen (falls vorhanden)
        if (shootSound != null)
            shootSound.Play();

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall") || collision.CompareTag("Obstacle") || collision.CompareTag("ground"))
        {
            Debug.Log("Enemy bullet hit an obstacle.");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            Debug.Log("Enemy bullet hit the player!");

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
