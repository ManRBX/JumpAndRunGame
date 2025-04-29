using UnityEngine;
using System.IO;

public class PlayerPrefsLoader : MonoBehaviour
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
        public PlayerPrefEntry[] entries;
    }

    void Awake()
    {
        string path = Application.persistentDataPath + "/playerprefs.json";
        if (!File.Exists(path))
        {
            Debug.Log("Keine gespeicherte PlayerPrefs-JSON gefunden.");
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<PlayerPrefsData>(json);

        foreach (var entry in data.entries)
        {
            switch (entry.type)
            {
                case "int":
                    PlayerPrefs.SetInt(entry.key, int.Parse(entry.value));
                    break;
                case "float":
                    PlayerPrefs.SetFloat(entry.key, float.Parse(entry.value));
                    break;
                case "string":
                    PlayerPrefs.SetString(entry.key, entry.value);
                    break;
            }
        }

        Debug.Log("PlayerPrefs aus JSON-Datei geladen.");
    }
}
