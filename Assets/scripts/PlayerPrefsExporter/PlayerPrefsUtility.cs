using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsUtility
{
    public static List<string> GetAllKeys()
    {
        return PlayerPrefsKeyTracker.GetAllTrackedKeys();
    }

    public static string GetValueType(string key)
    {
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
