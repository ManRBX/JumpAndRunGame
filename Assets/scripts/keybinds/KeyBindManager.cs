using UnityEngine;
using System;
using System.Collections.Generic;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance;

    // In diesem Dictionary speichere ich pro Aktion (z. B. "Jump") den zugehörigen KeyCode (z. B. KeyCode.Space).
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    // Liste aller Aktionen, die ich umbelegen möchte.
    private string[] allActions = new string[]
    {
        "Jump",
        "MoveLeft",
        "MoveRight",
        "ClimbUp",
        "ClimbDown",
        "DropPlatform",
        "Shoot"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Überlebt Szenenwechsel
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Standardwerte für jede Aktion festlegen
        keys["Jump"] = KeyCode.Space;
        keys["MoveLeft"] = KeyCode.A;
        keys["MoveRight"] = KeyCode.D;
        keys["ClimbUp"] = KeyCode.W;
        keys["ClimbDown"] = KeyCode.S;
        keys["DropPlatform"] = KeyCode.S;      // Zum Durchfallen durch OneWay-Plattformen
        keys["Shoot"] = KeyCode.Mouse0; // Linke Maustaste

        LoadKeysFromPrefs();
    }

    /// <summary>
    /// Gibt den KeyCode zurück, den du für die angegebene Aktion festgelegt hast.
    /// Beispiel: GetKeyCodeForAction("Jump") -> KeyCode.Space
    /// </summary>
    public KeyCode GetKeyCodeForAction(string actionName)
    {
        if (keys.ContainsKey(actionName))
            return keys[actionName];
        return KeyCode.None; // Falls die Aktion nicht existiert
    }

    /// <summary>
    /// Legt eine neue Taste für die angegebene Aktion fest und speichert das in PlayerPrefs.
    /// Beispiel: SetKey("DropPlatform", KeyCode.DownArrow)
    /// </summary>
    public void SetKey(string actionName, KeyCode newKey)
    {
        if (keys.ContainsKey(actionName))
        {
            keys[actionName] = newKey;
            Debug.Log($"Aktion '{actionName}' ist jetzt '{newKey}'.");
        }
        else
        {
            keys.Add(actionName, newKey);
            Debug.Log($"Neue Aktion '{actionName}' angelegt und auf '{newKey}' gesetzt.");
        }

        SaveKeysToPrefs();
    }

    /// <summary>
    /// Lädt alle Keybinds aus den PlayerPrefs, falls bereits gespeichert.
    /// </summary>
    private void LoadKeysFromPrefs()
    {
        foreach (string actionName in allActions)
        {
            string storedKeyString = PlayerPrefs.GetString(actionName, "");
            if (!string.IsNullOrEmpty(storedKeyString))
            {
                // Den String in einen KeyCode umwandeln
                if (Enum.TryParse(storedKeyString, out KeyCode parsedKey))
                {
                    keys[actionName] = parsedKey;
                }
            }
        }
    }

    /// <summary>
    /// Speichert alle Keybinds in PlayerPrefs (z. B. "Jump" -> "Space").
    /// </summary>
    private void SaveKeysToPrefs()
    {
        foreach (var kvp in keys)
        {
            PlayerPrefs.SetString(kvp.Key, kvp.Value.ToString());
        }
        PlayerPrefs.Save();
    }
}
