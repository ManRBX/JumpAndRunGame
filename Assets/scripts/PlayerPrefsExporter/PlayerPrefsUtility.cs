using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsUtility
{
    // Achtung: Funktioniert nur, wenn du die Keys manuell pflegst oder aus einer anderen Quelle holst
    public static List<string> GetAllKeys()
    {
        // Beispiel: Du könntest eine eigene Liste verwalten oder diese Keys hart coden
        return new List<string> { "HighScore", "Volume", "PlayerName" };
    }

    public static string GetValueType(string key)
    {
        // Reihenfolge ist wichtig, weil GetString auch für Zahlen funktionieren kann
        if (PlayerPrefs.HasKey(key))
        {
            int iVal = PlayerPrefs.GetInt(key, int.MinValue + 1);
            if (iVal != int.MinValue + 1) return "int";

            float fVal = PlayerPrefs.GetFloat(key, float.MinValue + 1f);
            if (fVal != float.MinValue + 1f) return "float";

            return "string";
        }
        return "string";
    }
}
