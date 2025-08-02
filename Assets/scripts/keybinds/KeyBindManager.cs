using System.Collections.Generic;
using UnityEngine;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance;

    private const string PlayerPrefPrefix = "Key_";

    // Speichert aktuelle Bindings
    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();

    // Standardbelegung
    private readonly Dictionary<string, KeyCode> defaultBindings = new Dictionary<string, KeyCode>()
    {
        { "Jump", KeyCode.Space },
        { "MoveLeft", KeyCode.A },
        { "MoveRight", KeyCode.D },
        { "ClimbUp", KeyCode.W },
        { "ClimbDown", KeyCode.S },
        { "DropPlatform", KeyCode.S },
        { "Shoot", KeyCode.Mouse0 },
        { "OpenInventory", KeyCode.C },
        { "OpenDoor", KeyCode.E },
        { "UseSlot2", KeyCode.Alpha2 },
        { "UseSlot3", KeyCode.Alpha3 },
        { "UseSlot4", KeyCode.Alpha4 }
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitOnLoad()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("KeyBindManager");
            Instance = obj.AddComponent<KeyBindManager>();
            DontDestroyOnLoad(obj);
        }
    }

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

    // Gibt den Key für eine Action zurück
    public KeyCode GetKeyCodeForAction(string action)
    {
        if (keyBindings.TryGetValue(action, out KeyCode key))
            return key;

        Debug.LogWarning($"❓ Aktion '{action}' nicht gefunden.");
        return KeyCode.None;
    }

    // Setzt eine neue Taste für eine Aktion
    public void SetKey(string action, KeyCode key)
    {
        if (!keyBindings.ContainsKey(action))
        {
            Debug.LogWarning($"⚠️ Aktion '{action}' ist nicht registriert.");
            return;
        }

        keyBindings[action] = key;
        PlayerPrefs.SetString(PlayerPrefPrefix + action, key.ToString());
        PlayerPrefsKeyTracker.TrackKey(PlayerPrefPrefix + action);
        PlayerPrefs.Save();
    }

    // Lädt Tasten aus PlayerPrefs oder setzt Standard
    private void LoadBindings()
    {
        keyBindings.Clear();

        foreach (var entry in defaultBindings)
        {
            string keyPref = PlayerPrefPrefix + entry.Key;

            if (PlayerPrefs.HasKey(keyPref))
            {
                string saved = PlayerPrefs.GetString(keyPref);
                if (System.Enum.TryParse(saved, out KeyCode parsedKey))
                {
                    keyBindings[entry.Key] = parsedKey;
                }
                else
                {
                    Debug.LogWarning($"❌ Ungültige Taste für {entry.Key}: {saved} → nutze Standard");
                    keyBindings[entry.Key] = entry.Value;
                }
            }
            else
            {
                keyBindings[entry.Key] = entry.Value;
            }

            PlayerPrefsKeyTracker.TrackKey(keyPref);
        }
    }

    // Setzt alle auf Standard zurück
    public void ResetToDefaults()
    {
        foreach (var entry in defaultBindings)
        {
            keyBindings[entry.Key] = entry.Value;
            PlayerPrefs.SetString(PlayerPrefPrefix + entry.Key, entry.Value.ToString());
            PlayerPrefsKeyTracker.TrackKey(PlayerPrefPrefix + entry.Key);
        }

        PlayerPrefs.Save();
        Debug.Log("🔁 Alle Tastenzuweisungen wurden zurückgesetzt.");
    }

    // Gibt alle Bindings zurück (z. B. fürs UI)
    public Dictionary<string, KeyCode> GetAllBindings()
    {
        return new Dictionary<string, KeyCode>(keyBindings);
    }
}
