using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;  // Keine Schwerkraft
        Destroy(gameObject, lifeTime);  // Bullet zerstört sich nach Zeit
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ground"))
        {
            Debug.Log("Bullet trifft Boden.");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Bullet trifft Gegner!");

            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);  // Gegner erleidet 1 Schaden

                if (enemy.health <= 0)
                {
                    int kills = PlayerPrefs.GetInt("EnemyKills", 0) + 1;
                    PlayerPrefs.SetInt("EnemyKills", kills);
                    PlayerPrefs.Save();

                    Debug.Log($"Gegner getötet! Gesamtzahl: {kills}");
                }
            }

            Destroy(gameObject);
        }
    }
}
