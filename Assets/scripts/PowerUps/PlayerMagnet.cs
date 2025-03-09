using UnityEngine;
using System.Collections;

public class PlayerMagnet : MonoBehaviour
{
    [Tooltip("Radius within which coins are attracted.")]
    public float magnetRadius = 5f;
    [Tooltip("Speed at which coins move toward the player.")]
    public float magnetForce = 5f;

    private bool magnetActive = false;
    private Coroutine magnetCoroutine;

    /// <summary>
    /// Wird vom MagnetPickup aufgerufen, wenn dieser eingesammelt wird.
    /// </summary>
    /// <param name="duration">Wie lange der Magnet-Effekt aktiv sein soll.</param>
    public void ActivateMagnet(float duration)
    {
        if (!magnetActive)
        {
            magnetCoroutine = StartCoroutine(MagnetRoutine(duration));
        }
    }

    /// <summary>
    /// Stoppt den Magnet-Effekt vorzeitig.
    /// </summary>
    public void DeactivateMagnet()
    {
        if (magnetActive)
        {
            // Stoppe den laufenden Magnet-Coroutine
            StopCoroutine(magnetCoroutine);
            magnetActive = false;
            Debug.Log("Magnet deactivated via DeactivateMagnet().");
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
        Debug.Log("Magnet effect duration ended.");
    }

    /// <summary>
    /// Zieht alle Coins im Umkreis 'magnetRadius' in Richtung Spieler.
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

    // Visualisierung des Magnetradius im Scene-View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
