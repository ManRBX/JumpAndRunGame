using UnityEngine;

public class TeleportOnKey : MonoBehaviour
{
    [Header("🔄 Teleport-Ziel")]
    public Transform teleportDestination;

    [Header("🕹️ Taste (Keybind: OpenDoor)")]
    public string actionName = "OpenDoor";

    [Header("🚪 Nur Spieler darf teleportieren")]
    public string playerTag = "Player";

    [Header("🔊 Teleport-Sound")]
    public AudioSource teleportSound;

    [Header("⏱️ Cooldown")]
    public float teleportCooldown = 5f;
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
        if (!isPlayerInZone || teleportDestination == null || player == null) return;

        KeyCode key = KeyBindManager.Instance.GetKeyCodeForAction(actionName);

        if (key != KeyCode.None && Input.GetKeyDown(key))
        {
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
