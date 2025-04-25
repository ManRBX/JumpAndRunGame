using UnityEngine;
using System.IO;

public class PlayerPrefsImporter : MonoBehaviour
{
    void Start()
    {
        string path = Application.persistentDataPath + "/playerprefs.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerPrefsData data = JsonUtility.FromJson<PlayerPrefsData>(json);

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

            Debug.Log("PlayerPrefs erfolgreich aus JSON geladen.");
        }
        else
        {
            Debug.Log("Keine PlayerPrefs-JSON gefunden.");
        }
    }
}
