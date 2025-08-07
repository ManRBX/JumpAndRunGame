using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;

    [Header("🔊 Schuss-Sound")]
    public AudioSource shootSound;

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        Destroy(gameObject, lifeTime);

        if (shootSound != null)
            shootSound.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ground") || collision.CompareTag("Wall") || collision.CompareTag("Obstacle") || collision.CompareTag("Ground"))
        {
            Debug.Log("Bullet hit an obstacle.");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Bullet hit an enemy!");

            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);

                if (enemy.health <= 0)
                {
                    int kills = PlayerPrefs.GetInt("EnemyKills", 0) + 1;
                    PlayerPrefs.SetInt("EnemyKills", kills);
                    PlayerPrefsKeyTracker.TrackKey("EnemyKills");
                    PlayerPrefs.Save();

                    Debug.Log($"Enemy killed! Total count: {kills}");
                }
            }

            Destroy(gameObject);
        }
    }
}
