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
    public float saveIntervalSeconds = 1f;

    private float nextAutoSaveTime;
    private float nextSilentSaveTime;

    [Header("UI für AutoSave")]
    public GameObject autoSaveUI;
    public TMP_Text autoSaveText;
    public Image autoSaveSpinner;

    private Coroutine uiRoutine;

    private void Start()
    {
        nextAutoSaveTime = Time.time + autoSaveIntervalMinutes * 60f;
        nextSilentSaveTime = Time.time + saveIntervalSeconds;

        if (autoSaveUI != null)
            autoSaveUI.SetActive(false);
    }

    private void Update()
    {
        if (Time.time >= nextSilentSaveTime)
        {
            SavePrefsToJson();
            nextSilentSaveTime = Time.time + saveIntervalSeconds;
        }

        if (Time.time >= nextAutoSaveTime)
        {
            SavePrefsToJsonWithUI();
            nextAutoSaveTime = Time.time + autoSaveIntervalMinutes * 60f;
        }

        if (autoSaveSpinner != null && autoSaveSpinner.gameObject.activeSelf)
        {
            autoSaveSpinner.transform.Rotate(Vector3.forward * -180f * Time.deltaTime);
        }
    }

    public void SavePrefsToJson()
    {
        SavePrefsToJsonInternal(showUI: false);
    }

    private void SavePrefsToJsonWithUI()
    {
        SavePrefsToJsonInternal(showUI: true);
    }

    private void SavePrefsToJsonInternal(bool showUI)
    {
        var keys = PlayerPrefsKeyTracker.GetAllTrackedKeys();
        var data = new PlayerPrefsData();

        foreach (string key in keys)
        {
            if (!PlayerPrefs.HasKey(key)) continue;

            string value = "";
            string type = "";

            // Sonderfall: Volume immer als float behandeln
            if (key == "Volume")
            {
                value = PlayerPrefs.GetFloat(key).ToString();
                type = "float";
            }
            else
            {
                // Fallback auf Float, Int, String
                float floatTest = PlayerPrefs.GetFloat(key, float.NaN);
                if (!float.IsNaN(floatTest) && floatTest != 0f)
                {
                    value = floatTest.ToString();
                    type = "float";
                }
                else
                {
                    int intTest = PlayerPrefs.GetInt(key, int.MinValue);
                    if (intTest != int.MinValue)
                    {
                        value = intTest.ToString();
                        type = "int";
                    }
                    else
                    {
                        value = PlayerPrefs.GetString(key, "");
                        type = "string";
                    }
                }
            }

            data.entries.Add(new PlayerPrefEntry { key = key, type = type, value = value });
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "playerprefs.json");
        File.WriteAllText(path, json);

        if (showUI)
        {
            if (uiRoutine != null) StopCoroutine(uiRoutine);
            uiRoutine = StartCoroutine(ShowSaveUI());
        }
    }

    private IEnumerator ShowSaveUI()
    {
        if (autoSaveUI != null)
            autoSaveUI.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        if (autoSaveUI != null)
            autoSaveUI.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        SavePrefsToJson();
    }
}
