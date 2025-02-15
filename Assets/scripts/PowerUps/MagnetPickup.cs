using UnityEngine;

public class MagnetPickup : MonoBehaviour
{
    [Tooltip("How long the magnet effect lasts (in seconds).")]
    public float duration = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Check if there's a 'PlayerMagnet' component
            PlayerMagnet playerMagnet = collision.GetComponent<PlayerMagnet>();
            if (playerMagnet != null)
            {
                playerMagnet.ActivateMagnet(duration);
            }

            Destroy(gameObject);
        }
    }
}
