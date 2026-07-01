using UnityEngine;
using TMPro;

public class CharacterUnlockPickup : MonoBehaviour
{
    [Header("🎭 Charakter Unlock")]
    public int characterIndexToUnlock = 1;
    public string characterName = "Nova";
    public string unlockKeyPrefix = "CharacterUnlocked_";

    [Header("🧾 UI Optional")]
    public TMP_Text unlockMessageText;
    public float messageDuration = 3f;
    public string unlockMessage = "Neuer Charakter freigeschaltet: {name}!";

    [Header("🔊 Sound Optional")]
    public AudioSource unlockSound;

    [Header("✨ Effekt Optional")]
    public GameObject unlockEffect;

    [Header("⚙️ Verhalten")]
    public bool hideIfAlreadyUnlocked = true;
    public bool disableInsteadOfDestroy = true;

    private string UnlockKey => unlockKeyPrefix + characterIndexToUnlock;

    private void Start()
    {
        if (hideIfAlreadyUnlocked && PlayerPrefs.GetInt(UnlockKey, 0) == 1)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        UnlockCharacter();
    }

    private void UnlockCharacter()
    {
        // Unlock speichern
        PlayerPrefs.SetInt(UnlockKey, 1);
        PlayerPrefsKeyTracker.TrackKey(UnlockKey);

        // Unlock-Zeitstempel speichern
        string timeKey = UnlockKey + "_Time";
        PlayerPrefs.SetString(timeKey, System.DateTime.Now.ToString());
        PlayerPrefsKeyTracker.TrackKey(timeKey);

        // Unlock-Name speichern
        string nameKey = UnlockKey + "_Name";
        PlayerPrefs.SetString(nameKey, characterName);
        PlayerPrefsKeyTracker.TrackKey(nameKey);

        PlayerPrefs.Save();

        Debug.Log($"✅ Charakter freigeschaltet: {characterName} | Index {characterIndexToUnlock} | Key: {UnlockKey}");

        if (unlockSound != null)
            unlockSound.Play();

        if (unlockEffect != null)
            Instantiate(unlockEffect, transform.position, Quaternion.identity);

        if (unlockMessageText != null)
        {
            string msg = unlockMessage.Replace("{name}", characterName);
            unlockMessageText.text = msg;
            unlockMessageText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDuration);
        }

        if (disableInsteadOfDestroy)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }

    private void HideMessage()
    {
        if (unlockMessageText != null)
            unlockMessageText.gameObject.SetActive(false);
    }

    // Statische Hilfsmethode: prüfen ob ein Character freigeschaltet ist
    public static bool IsUnlocked(string prefix, int index)
    {
        return PlayerPrefs.GetInt(prefix + index, 0) == 1;
    }
}