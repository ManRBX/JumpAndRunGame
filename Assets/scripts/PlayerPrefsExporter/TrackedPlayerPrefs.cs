using UnityEngine;
using System.Collections.Generic;

public static class TrackedPlayerPrefs
{
    private const string KeyListKey = "__TrackedKeys"; // interner Speicher für Keyliste

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        AddKey(key);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        AddKey(key);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        AddKey(key);
    }

    public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
    public static float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);
    public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

    public static List<string> GetAllTrackedKeys()
    {
        string raw = PlayerPrefs.GetString(KeyListKey, "");
        return new List<string>(raw.Split('|', System.StringSplitOptions.RemoveEmptyEntries));
    }

    private static void AddKey(string key)
    {
        var keys = GetAllTrackedKeys();
        if (!keys.Contains(key))
        {
            keys.Add(key);
            string joined = string.Join("|", keys);
            PlayerPrefs.SetString(KeyListKey, joined);
        }
    }
}
