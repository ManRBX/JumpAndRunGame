using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;  // No gravity
        Destroy(gameObject, lifeTime);  // Bullet destroys itself after a set time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ground"))
        {
            Debug.Log("Bullet hit the ground.");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Bullet hit an enemy!");

            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);  // Enemy takes 1 damage

                if (enemy.health <= 0)
                {
                    int kills = PlayerPrefs.GetInt("EnemyKills", 0) + 1;
                    PlayerPrefs.SetInt("EnemyKills", kills);
                    PlayerPrefs.Save();

                    Debug.Log($"Enemy killed! Total count: {kills}");
                }
            }

            Destroy(gameObject);
        }
    }
}
