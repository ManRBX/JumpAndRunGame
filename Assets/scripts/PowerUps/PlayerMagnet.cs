using UnityEngine;
using System.Collections;

public class PlayerMagnet : MonoBehaviour
{
    [Tooltip("Radius within which coins are attracted.")]
    public float magnetRadius = 5f;
    [Tooltip("Speed at which coins move toward the player.")]
    public float magnetForce = 5f;

    private bool magnetActive = false;

    /// <summary>
    /// Called by MagnetPickup when collected.
    /// </summary>
    public void ActivateMagnet(float duration)
    {
        if (!magnetActive)
        {
            StartCoroutine(MagnetRoutine(duration));
        }
    }

    private IEnumerator MagnetRoutine(float duration)
    {
        magnetActive = true;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            AttractCoins();
            yield return null;
        }

        magnetActive = false;
    }

    /// <summary>
    /// Pull all coins within 'magnetRadius' toward the player.
    /// </summary>
    private void AttractCoins()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, magnetRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Coin"))
            {
                Vector3 direction = (transform.position - hit.transform.position).normalized;
                hit.transform.position += direction * magnetForce * Time.deltaTime;
            }
        }
    }

    // Optional: visualize the magnet radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
