using UnityEngine;
using TMPro;
using System.Collections;

public class BonusUnlockTrigger : MonoBehaviour
{
    [Header("Was soll freigeschaltet werden?")]
    public string[] unlockKeys; // z. B. "LevelBonus01"

    [Header("UI Anzeige")]
    public TMP_Text unlockMessageText;
    public float messageDuration = 3f;

    [Header("Einstellungen")]
    public bool onlyOnce = true;

    private bool alreadyUnlocked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (onlyOnce && alreadyUnlocked) return;

        alreadyUnlocked = true;

        foreach (string baseKey in unlockKeys)
        {
            string unlockKey = baseKey + "_Unlocked";
            string visitKey = baseKey + "_Visited";

            PlayerPrefs.SetInt(unlockKey, 1);
            PlayerPrefs.SetInt(visitKey, 1);

            PlayerPrefsKeyTracker.TrackKey(unlockKey);
            PlayerPrefsKeyTracker.TrackKey(visitKey);

            Debug.Log("🔓 Freigeschaltet & besucht: " + baseKey);
        }

        PlayerPrefs.Save();

        // JSON Save
        PlayerPrefsSaver saver = FindObjectOfType<PlayerPrefsSaver>();
        if (saver != null) saver.SavePrefsToJson();

        ShowMessage();
    }

    void ShowMessage()
    {
        if (unlockMessageText != null && unlockKeys.Length > 0)
        {
            string joinedKeys = string.Join(", ", unlockKeys);
            unlockMessageText.text = "🎉 Freigeschaltet: " + joinedKeys;
            unlockMessageText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDuration);
        }
    }

    void HideMessage()
    {
        if (unlockMessageText != null)
            unlockMessageText.gameObject.SetActive(false);
    }
}
