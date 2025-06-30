using UnityEngine;

public class TeleportOnKey : MonoBehaviour
{
    [Header("Teleport-Ziel")]
    public Transform teleportDestination;

    [Header("Taste für Teleport")]
    public KeyCode teleportKey = KeyCode.E;

    [Header("Nur Spieler darf teleportieren")]
    public string playerTag = "Player";

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
            player.transform.position = teleportDestination.position;
        }
    }
}
