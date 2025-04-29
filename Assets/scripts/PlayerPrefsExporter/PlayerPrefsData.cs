using System.Collections.Generic;

[System.Serializable]
public class PlayerPrefEntry
{
    public string key;
    public string type; // "int", "float", "string"
    public string value;
}

[System.Serializable]
public class PlayerPrefsData
{
    public List<PlayerPrefEntry> entries = new List<PlayerPrefEntry>();
}
