using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayerPrefsExporter : MonoBehaviour
{
    public void SaveAllPlayerPrefsToJson()
    {
        PlayerPrefsData data = new PlayerPrefsData();
        foreach (var key in PlayerPrefsUtility.GetAllKeys())
        {
            string type = PlayerPrefsUtility.GetValueType(key);
            string value = "";

            switch (type)
            {
                case "int":
                    value = PlayerPrefs.GetInt(key).ToString();
                    break;
                case "float":
                    value = PlayerPrefs.GetFloat(key).ToString();
                    break;
                case "string":
                    value = PlayerPrefs.GetString(key);
                    break;
            }

            data.entries.Add(new PlayerPrefEntry { key = key, type = type, value = value });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/playerprefs.json", json);
        Debug.Log("Alle PlayerPrefs wurden als JSON gespeichert!");
    }
}
