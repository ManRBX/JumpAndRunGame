using UnityEngine;

public class TeleportOnKey : MonoBehaviour
{
    [Header("🔄 Teleport-Ziel")]
    public Transform teleportDestination;

    [Header("🕹️ Taste für Teleport")]
    public KeyCode teleportKey = KeyCode.E;

    [Header("🚪 Nur Spieler darf teleportieren")]
    public string playerTag = "Player";

    [Header("🔊 Teleport-Sound")]
    public AudioSource teleportSound;

    [Header("⏱️ Cooldown")]
    public float teleportCooldown = 5f; // Sekunden
    private float lastTeleportTime = -Mathf.Infinity;

    private bool isPlayerInZone = false;
    private GameObject player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInZone = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInZone = false;
            player = null;
        }
    }

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(teleportKey) && teleportDestination != null && player != null)
        {
            // Nur wenn der Cooldown abgelaufen ist
            if (Time.time >= lastTeleportTime + teleportCooldown)
            {
                player.transform.position = teleportDestination.position;

                if (teleportSound != null)
                    teleportSound.Play();

                lastTeleportTime = Time.time;
            }
            else
            {
                Debug.Log("⏳ Teleport noch im Cooldown!");
            }
        }
    }
}
