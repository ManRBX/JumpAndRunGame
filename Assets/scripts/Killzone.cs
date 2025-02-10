using UnityEngine;

public class Killzone : MonoBehaviour
{
    public int damage = 100;  // Schaden, der in der Killzone verursacht wird (z.B. tödlich)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // Spieler erleidet vollen Schaden
            }
        }
    }
}
