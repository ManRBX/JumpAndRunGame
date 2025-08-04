using UnityEngine;
using TMPro;

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

    [Header("🔑 Schlüssel Einstellungen")]
    public bool requiresKey = false;
    public string requiredKeyName;
    public TMP_Text noKeyMessageText;
    public string noKeyMessage = "Du benötigst einen Schlüssel!";
    public float messageDisplayDuration = 3f;

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
            if (Time.time < lastTeleportTime + teleportCooldown)
            {
                Debug.Log("⏳ Teleport noch im Cooldown!");
                return;
            }

            if (requiresKey && PlayerPrefs.GetInt(requiredKeyName, 0) == 0)
            {
                if (noKeyMessageText != null)
                    StartCoroutine(ShowNoKeyMessage());

                Debug.Log(noKeyMessage);
                return;
            }

            player.transform.position = teleportDestination.position;

            if (teleportSound != null)
                teleportSound.Play();

            lastTeleportTime = Time.time;
        }
    }

    private System.Collections.IEnumerator ShowNoKeyMessage()
    {
        noKeyMessageText.text = noKeyMessage;
        noKeyMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDisplayDuration);

        noKeyMessageText.gameObject.SetActive(false);
    }
}