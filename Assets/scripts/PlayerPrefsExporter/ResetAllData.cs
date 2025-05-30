using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Steamworks;

public class ResetAllData : MonoBehaviour
{
    [Header("Optional: Bestätigungs-Popup")]
    public GameObject confirmationPopup;

    [Header("JSON-Dateiname (z. B. playerprefs.json)")]
    public string jsonFileName = "playerprefs.json";

    public void OnClick_ResetEverything()
    {
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(true);
        }
        else
        {
            DoFullReset();
        }
    }

    public void ConfirmReset()
    {
        DoFullReset();

        if (confirmationPopup != null)
            confirmationPopup.SetActive(false);
    }

    public void CancelReset()
    {
        if (confirmationPopup != null)
            confirmationPopup.SetActive(false);
    }

    private void DoFullReset()
    {
        // 🎮 Steam Achievements und Stats zurücksetzen
        if (SteamManager.Initialized)
        {
            SteamUserStats.ResetAllStats(true); // true = auch Achievements
            SteamUserStats.StoreStats();
            Debug.Log("🔥 Steam-Achievements und Stats wurden zurückgesetzt.");
        }
        else
        {
            Debug.LogWarning("⚠️ Steam nicht initialisiert – Achievements nicht zurückgesetzt.");
        }

        // 🧹 Alle PlayerPrefs löschen
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🧹 Alle PlayerPrefs wurden gelöscht.");

        // 🗑️ JSON-Datei löschen
        string jsonPath = Path.Combine(Application.persistentDataPath, jsonFileName);
        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
            Debug.Log("🗑️ JSON-Datei gelöscht: " + jsonPath);
        }
        else
        {
            Debug.Log("ℹ️ Keine JSON-Datei gefunden unter: " + jsonPath);
        }

        // 📦 Direkt danach neu speichern (leerer Stand)
        var saver = FindObjectOfType<PlayerPrefsSaver>();
        if (saver != null)
        {
            saver.SavePrefsToJson();
            Debug.Log("💾 Nach Reset neu gespeichert.");
        }
        else
        {
            Debug.LogWarning("❌ Kein PlayerPrefsSaver gefunden – konnte nicht speichern.");
        }

        // 🔁 Szene neu laden
        ResetSession.wasReset = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
