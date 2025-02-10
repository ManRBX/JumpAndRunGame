using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 5;  // Wie viel Munition wird hinzugefügt

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerShooting playerShooting = collision.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                playerShooting.AddAmmo(ammoAmount);
                Destroy(gameObject);  // Munition nach dem Einsammeln entfernen
            }
        }
    }
}
