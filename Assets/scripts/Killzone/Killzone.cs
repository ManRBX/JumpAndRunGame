using UnityEngine;

public class Killzone : MonoBehaviour
{
    public int damage = 100;
    public float damageCooldown = 1f;

    [Tooltip("KillZone = sofortiger Tod | Spike = normaler Schaden")]
    public string damageSourceTag = "Spike";

    private float lastDamageTime = -999f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamage(collision);
    }

    private void TryDamage(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, damageSourceTag);
            lastDamageTime = Time.time;
        }
    }
}