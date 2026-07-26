using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float fireRate = 1f;
    public float shootRange = 15f;

    [Header("Players")]
    [Tooltip("Alle Player-Charaktere hier eintragen.")]
    public Transform[] players;

    private Transform currentPlayer;
    private float nextFireTime = 0f;

    private void Update()
    {
        FindActivePlayer();

        if (currentPlayer == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, currentPlayer.position);

        if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FindActivePlayer()
    {
        currentPlayer = null;

        if (players == null || players.Length == 0)
            return;

        foreach (Transform player in players)
        {
            if (player != null && player.gameObject.activeInHierarchy)
            {
                currentPlayer = player;
                return;
            }
        }
    }

    private void Shoot()
    {
        if (currentPlayer == null || bulletPrefab == null || firePoint == null)
            return;

        Vector2 direction = (currentPlayer.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }

        Debug.Log("Enemy fired a shot at: " + currentPlayer.name);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null)
            return;

        // Schussreichweite
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        // Flugrichtung anzeigen
        Gizmos.color = Color.yellow;

        Vector2 direction = Vector2.right;

        if (Application.isPlaying)
        {
            if (currentPlayer != null)
                direction = (currentPlayer.position - firePoint.position).normalized;
        }
        else
        {
            if (players != null)
            {
                foreach (Transform player in players)
                {
                    if (player != null)
                    {
                        direction = (player.position - firePoint.position).normalized;
                        break;
                    }
                }
            }
        }

        float estimatedFlightTime = 1f;
        float estimatedDistance = bulletForce * estimatedFlightTime;

        Gizmos.DrawLine(
            firePoint.position,
            firePoint.position + (Vector3)(direction * estimatedDistance)
        );

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            firePoint.position + (Vector3)(direction * estimatedDistance * 0.5f),
            $"~{estimatedDistance:F1} Units"
        );
#endif
    }
}