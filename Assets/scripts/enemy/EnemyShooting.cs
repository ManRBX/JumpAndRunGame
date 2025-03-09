using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;      // Your bullet prefab for the enemy
    public Transform firePoint;          // The position from which shots are fired
    public float bulletForce = 20f;      // The force with which the projectile is fired
    public float fireRate = 1f;          // Time between shots (in seconds)
    public float shootRange = 15f;       // The enemy will only shoot if the player is within this range
    public Transform player;             // Reference to the player (assign in the Inspector)

    private float nextFireTime = 0f;     // Timer to ensure shooting intervals

    void Update()
    {
        // If the player exists, check the distance to the player
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
        // Determine the direction from the firePoint to the player
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Optional: Rotate the projectile to face the shooting direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate the projectile
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }

        Debug.Log("Enemy fired a shot in direction: " + direction);
    }
}
