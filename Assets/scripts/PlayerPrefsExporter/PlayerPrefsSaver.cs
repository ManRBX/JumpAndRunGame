using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PlayerPrefsSaver : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPrefEntry
    {
        public string key;
        public string type;
        public string value;
    }

    [System.Serializable]
    public class PlayerPrefsData
    {
        public List<PlayerPrefEntry> entries = new List<PlayerPrefEntry>();
    }

    public void SavePrefsToJson()
    {
        var keys = PlayerPrefsKeyTracker.GetAllTrackedKeys(); // << Hier rufst du deinen Tracker auf
        var data = new PlayerPrefsData();

        Debug.Log("📦 Getrackte Keys: " + keys.Count);
        foreach (string key in keys)
        {
            if (!PlayerPrefs.HasKey(key)) continue;

            string value;
            string type;

            try
            {
                int i = PlayerPrefs.GetInt(key);
                value = i.ToString();
                type = "int";
            }
            catch
            {
                try
                {
                    float f = PlayerPrefs.GetFloat(key);
                    value = f.ToString();
                    type = "float";
                }
                catch
                {
                    value = PlayerPrefs.GetString(key);
                    type = "string";
                }
            }

            data.entries.Add(new PlayerPrefEntry { key = key, type = type, value = value });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/playerprefs.json", json);
        Debug.Log("✅ PlayerPrefs gespeichert unter:\n" + Application.persistentDataPath + "/playerprefs.json");
    }

    private void OnApplicationQuit()
    {
        SavePrefsToJson();
    }
}
