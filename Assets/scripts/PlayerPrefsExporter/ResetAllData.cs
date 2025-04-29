using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("🧹 Alle PlayerPrefs wurden gelöscht.");

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

        ResetSession.wasReset = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
