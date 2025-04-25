#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class PlayerPrefsKeys
{
    public static List<string> GetAllKeys()
    {
        List<string> keys = new List<string>();

        string prefsFile = $"{Application.persistentDataPath}/../UnityEditorPrefs.dat";
        if (!File.Exists(prefsFile))
        {
            foreach (var line in File.ReadAllLines(prefsFile))
            {
                if (line.StartsWith("key:"))
                {
                    string key = line.Substring(4);
                    keys.Add(key);
                }
            }
        }

        // Fallback – du kannst hier auch Keys hart codieren, falls du willst.
        keys.AddRange(new string[]
        {
            "GlobalPoints", "GlobalCoins", "GlobalAmmo", "Volume", "Fullscreen", "SelectedLanguage",
            "ShotsFired", "EnemyKills", "TotalCoins"
        });

        return keys;
    }
}
#endif
