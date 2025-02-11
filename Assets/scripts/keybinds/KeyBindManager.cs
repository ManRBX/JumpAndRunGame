using UnityEngine;
using System;
using System.Collections.Generic;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance;

    // I store the KeyCode for each action (e.g., "Jump" -> KeyCode.Space) in this dictionary.
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    // List of all actions I want to remap.
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
            DontDestroyOnLoad(gameObject);  // Survives scene transitions
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set default values for each action
        keys["Jump"] = KeyCode.Space;
        keys["MoveLeft"] = KeyCode.A;
        keys["MoveRight"] = KeyCode.D;
        keys["ClimbUp"] = KeyCode.W;
        keys["ClimbDown"] = KeyCode.S;
        keys["DropPlatform"] = KeyCode.S;      // To fall through one-way platforms
        keys["Shoot"] = KeyCode.Mouse0; // Left mouse button

        LoadKeysFromPrefs();
    }

    /// <summary>
    /// Returns the KeyCode set for the specified action.
    /// Example: GetKeyCodeForAction("Jump") -> KeyCode.Space
    /// </summary>
    public KeyCode GetKeyCodeForAction(string actionName)
    {
        if (keys.ContainsKey(actionName))
            return keys[actionName];
        return KeyCode.None; // If the action doesn't exist
    }

    /// <summary>
    /// Sets a new key for the specified action and saves it in PlayerPrefs.
    /// Example: SetKey("DropPlatform", KeyCode.DownArrow)
    /// </summary>
    public void SetKey(string actionName, KeyCode newKey)
    {
        if (keys.ContainsKey(actionName))
        {
            keys[actionName] = newKey;
            Debug.Log($"Action '{actionName}' is now set to '{newKey}'.");
        }
        else
        {
            keys.Add(actionName, newKey);
            Debug.Log($"New action '{actionName}' created and set to '{newKey}'.");
        }

        SaveKeysToPrefs();
    }

    /// <summary>
    /// Loads all keybinds from PlayerPrefs if already saved.
    /// </summary>
    private void LoadKeysFromPrefs()
    {
        foreach (string actionName in allActions)
        {
            string storedKeyString = PlayerPrefs.GetString(actionName, "");
            if (!string.IsNullOrEmpty(storedKeyString))
            {
                // Convert the string to a KeyCode
                if (Enum.TryParse(storedKeyString, out KeyCode parsedKey))
                {
                    keys[actionName] = parsedKey;
                }
            }
        }
    }

    /// <summary>
    /// Saves all keybinds in PlayerPrefs (e.g., "Jump" -> "Space").
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
