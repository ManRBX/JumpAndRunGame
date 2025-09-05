using UnityEngine;
using TMPro;
using System.Collections;

public class BonusUnlockTrigger : MonoBehaviour
{
    [Header("Was soll freigeschaltet werden?")]
    [Tooltip("Z. B. LevelBonus01 – wird intern als LevelBonus01_Unlocked / _Visited gespeichert.")]
    public string[] unlockKeys;

    [Header("UI Anzeige")]
    public TMP_Text unlockMessageText;
    [Tooltip("Der Text, der angezeigt wird (statt der Keys).")]
    [TextArea]
    public string customUnlockMessage = "🎉 Bonus-Level freigeschaltet!";
    public float messageDuration = 3f;

    [Header("Einstellungen")]
    public bool onlyOnce = true;

    private bool alreadyUnlocked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (onlyOnce && alreadyUnlocked) return;

        alreadyUnlocked = true;

        // ✅ Speichern der Flags (aber keine Keys anzeigen)
        foreach (string baseKey in unlockKeys)
        {
            if (string.IsNullOrWhiteSpace(baseKey)) continue;

            string unlockKey = baseKey + "_Unlocked";
            string visitKey = baseKey + "_Visited";

            PlayerPrefs.SetInt(unlockKey, 1);
            PlayerPrefs.SetInt(visitKey, 1);

            // Falls du einen KeyTracker hast:
            PlayerPrefsKeyTracker.TrackKey(unlockKey);
            PlayerPrefsKeyTracker.TrackKey(visitKey);

            Debug.Log("🔓 Freigeschaltet & besucht (gespeichert): " + baseKey);
        }

        PlayerPrefs.Save();

        // Optional: JSON Save
        PlayerPrefsSaver saver = FindObjectOfType<PlayerPrefsSaver>();
        if (saver != null) saver.SavePrefsToJson();

        // ✅ Zeige NUR deinen Custom-Text, nicht die Keys
        ShowMessage();
    }

    private void ShowMessage()
    {
        if (unlockMessageText != null && !string.IsNullOrEmpty(customUnlockMessage))
        {
            unlockMessageText.text = customUnlockMessage;
            unlockMessageText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDuration);
        }
    }

    private void HideMessage()
    {
        if (unlockMessageText != null)
            unlockMessageText.gameObject.SetActive(false);
    }
}
