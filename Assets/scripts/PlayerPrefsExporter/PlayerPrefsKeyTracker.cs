using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsKeyTracker
{
    private const string KeyListStorage = "__AllUsedKeys";

    public static void TrackKey(string key)
    {
        if (!PlayerPrefs.HasKey(KeyListStorage))
        {
            PlayerPrefs.SetString(KeyListStorage, key);
        }
        else
        {
            string allKeys = PlayerPrefs.GetString(KeyListStorage);
            List<string> keys = new List<string>(allKeys.Split('|'));
            if (!keys.Contains(key))
            {
                keys.Add(key);
                PlayerPrefs.SetString(KeyListStorage, string.Join("|", keys));
            }
        }

        PlayerPrefs.Save(); // 🔥 GANZ WICHTIG damit's dauerhaft gespeichert wird
    }

    public static List<string> GetAllTrackedKeys()
    {
        if (!PlayerPrefs.HasKey(KeyListStorage))
            return new List<string>();

        string raw = PlayerPrefs.GetString(KeyListStorage);
        return new List<string>(raw.Split('|'));
    }
}
