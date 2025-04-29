using UnityEngine;
using System.Collections.Generic;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance;

    private const string PlayerPrefPrefix = "Key_";

    // Alle Aktionen & die zugehörigen KeyCodes
    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();

    // Standardbelegungen – hier kannst du jederzeit neue Aktionen hinzufügen
    private readonly Dictionary<string, KeyCode> defaultBindings = new Dictionary<string, KeyCode>()
    {
        { "Jump", KeyCode.Space },
        { "MoveLeft", KeyCode.A },
        { "MoveRight", KeyCode.D },
        { "ClimbUp", KeyCode.W },
        { "ClimbDown", KeyCode.S },
        { "DropPlatform", KeyCode.S },   // Optional gleiche Taste wie ClimbDown
        { "Shoot", KeyCode.Mouse0 }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadBindings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gibt den aktuell zugewiesenen KeyCode für eine Aktion zurück.
    /// </summary>
    public KeyCode GetKeyCodeForAction(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];

        Debug.LogWarning($"❓ Action '{action}' nicht gefunden. Gib einen gültigen Namen an.");
        return KeyCode.None;
    }

    /// <summary>
    /// Setzt eine neue Taste für eine Aktion und speichert sie.
    /// </summary>
    public void SetKey(string action, KeyCode key)
    {
        if (keyBindings.ContainsKey(action))
        {
            keyBindings[action] = key;
            PlayerPrefs.SetString(PlayerPrefPrefix + action, key.ToString());
            PlayerPrefsKeyTracker.TrackKey(PlayerPrefPrefix + action);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning($"❓ Aktion '{action}' ist nicht registriert.");
        }
    }

    /// <summary>
    /// Lädt alle gespeicherten Bindings oder setzt sie auf Standard.
    /// </summary>
    private void LoadBindings()
    {
        foreach (var entry in defaultBindings)
        {
            string keyName = PlayerPrefPrefix + entry.Key;

            if (PlayerPrefs.HasKey(keyName))
            {
                string savedKey = PlayerPrefs.GetString(keyName);
                if (System.Enum.TryParse(savedKey, out KeyCode parsedKey))
                {
                    keyBindings[entry.Key] = parsedKey;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Ungültiger gespeicherter Key für {entry.Key}: {savedKey}. Setze Standard.");
                    keyBindings[entry.Key] = entry.Value;
                }
            }
            else
            {
                keyBindings[entry.Key] = entry.Value;
            }

            PlayerPrefsKeyTracker.TrackKey(keyName);
        }
    }

    /// <summary>
    /// Setzt alle Tastenzuweisungen auf ihre Standardwerte zurück.
    /// </summary>
    public void ResetToDefaults()
    {
        foreach (var entry in defaultBindings)
        {
            keyBindings[entry.Key] = entry.Value;
            PlayerPrefs.SetString(PlayerPrefPrefix + entry.Key, entry.Value.ToString());
            PlayerPrefsKeyTracker.TrackKey(PlayerPrefPrefix + entry.Key);
        }

        PlayerPrefs.Save();
        Debug.Log("🔄 Alle Tasten wurden auf Standard zurückgesetzt.");
    }

    /// <summary>
    /// Gibt alle aktuellen Bindings zurück (z. B. für Debug oder Export).
    /// </summary>
    public Dictionary<string, KeyCode> GetAllBindings()
    {
        return new Dictionary<string, KeyCode>(keyBindings);
    }
}
