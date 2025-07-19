using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float fireRate = 1f;
    public float shootRange = 15f;
    public Transform player;

    private float nextFireTime = 0f;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }

        Debug.Log("Enemy fired a shot in direction: " + direction);
    }

    void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        // 1. Sichtbarer Schussbereich (shootRange)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        // 2. Geschätzte Flugdistanz anhand der bulletForce
        Gizmos.color = Color.yellow;
        Vector2 direction = Vector2.right; // Default-Richtung im Editor
#if UNITY_EDITOR
        if (!Application.isPlaying && player != null)
        {
            direction = (player.position - firePoint.position).normalized;
        }
        else if (Application.isPlaying && player != null)
        {
            direction = (player.position - firePoint.position).normalized;
        }
#endif
        float estimatedFlightTime = 1.0f; // Sekunden geschätzt, kann angepasst werden
        float estimatedDistance = bulletForce * estimatedFlightTime;
        Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)(direction * estimatedDistance));

        // Optional: Info-Label anzeigen (Editor only)
#if UNITY_EDITOR
        UnityEditor.Handles.Label(firePoint.position + (Vector3)(direction * estimatedDistance * 0.5f), $"~{estimatedDistance:F1} Units");
#endif
    }
}
