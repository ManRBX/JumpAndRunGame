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

    [Header("📜 Nachrichten")]
    public TMP_Text messageText;
    public string noKeyMessage = "Du benötigst einen Schlüssel!";
    public string hasKeyMessage = "Du hast einen Schlüssel! Drücke E um hindurch zu gehen.";
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

            // Wenn Key vorhanden und Nachricht eingestellt → Sofort anzeigen
            if (requiresKey && PlayerPrefs.GetInt(requiredKeyName, 0) == 1)
            {
                ShowMessage(hasKeyMessage, true); // true = dauerhaft anzeigen, solange in Zone
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInZone = false;
            player = null;

            // Nachricht ausblenden
            if (messageText != null)
                messageText.gameObject.SetActive(false);
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
                if (messageText != null)
                    StartCoroutine(ShowTemporaryMessage(noKeyMessage));

                Debug.Log(noKeyMessage);
                return;
            }

            player.transform.position = teleportDestination.position;

            if (teleportSound != null)
                teleportSound.Play();

            lastTeleportTime = Time.time;
        }
    }

    private System.Collections.IEnumerator ShowTemporaryMessage(string text)
    {
        ShowMessage(text, false);
        yield return new WaitForSeconds(messageDisplayDuration);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void ShowMessage(string text, bool keepVisible)
    {
        if (messageText != null)
        {
            messageText.text = text;
            messageText.gameObject.SetActive(true);

            if (!keepVisible)
                StartCoroutine(HideAfterDelay());
        }
    }

    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(messageDisplayDuration);
        if (messageText != null && isPlayerInZone == false)
            messageText.gameObject.SetActive(false);
    }
}
