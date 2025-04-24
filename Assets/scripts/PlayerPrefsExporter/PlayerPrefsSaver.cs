using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Auto-Save Einstellungen")]
    public float autoSaveIntervalMinutes = 2f;
    private float nextAutoSaveTime;

    [Header("UI für AutoSave")]
    public GameObject autoSaveUI;          // Parent-Objekt mit Text + Icon
    public TMP_Text autoSaveText;          // Z.B. "Spielstand wird gespeichert..."
    public Image autoSaveSpinner;          // Drehendes Icon

    private Coroutine uiRoutine;

    private void Start()
    {
        nextAutoSaveTime = Time.time + autoSaveIntervalMinutes * 60f;

        if (autoSaveUI != null)
            autoSaveUI.SetActive(false);
    }

    private void Update()
    {
        if (Time.time >= nextAutoSaveTime)
        {
            SavePrefsToJson();
            nextAutoSaveTime = Time.time + autoSaveIntervalMinutes * 60f;
        }

        // Rotation vom Spinner
        if (autoSaveSpinner != null && autoSaveSpinner.gameObject.activeSelf)
        {
            autoSaveSpinner.transform.Rotate(Vector3.forward * -180 * Time.deltaTime);
        }
    }

    public void SavePrefsToJson()
    {
        var keys = PlayerPrefsKeyTracker.GetAllTrackedKeys();
        var data = new PlayerPrefsData();

        Debug.Log("📦 Getrackte Keys: " + keys.Count);
        foreach (string key in keys)
        {
            if (!PlayerPrefs.HasKey(key)) continue;

            string value;
            string type;

            try { value = PlayerPrefs.GetInt(key).ToString(); type = "int"; }
            catch
            {
                try { value = PlayerPrefs.GetFloat(key).ToString(); type = "float"; }
                catch { value = PlayerPrefs.GetString(key); type = "string"; }
            }

            data.entries.Add(new PlayerPrefEntry { key = key, type = type, value = value });
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/playerprefs.json";
        File.WriteAllText(path, json);

        Debug.Log("✅ PlayerPrefs gespeichert unter:\n" + path);

        if (uiRoutine != null) StopCoroutine(uiRoutine);
        uiRoutine = StartCoroutine(ShowSaveUI());
    }

    private IEnumerator ShowSaveUI()
    {
        if (autoSaveUI != null)
        {
            autoSaveUI.SetActive(true);
        }

        if (autoSaveText != null)
        {
            autoSaveText.text = "💾 Spielstand wird gespeichert...";
        }

        yield return new WaitForSeconds(2.5f);

        if (autoSaveUI != null)
        {
            autoSaveUI.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        SavePrefsToJson();
    }
}
