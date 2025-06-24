using UnityEngine;
using TMPro;  // Für den TextMeshPro Text

public class PlayerHealthUI : MonoBehaviour
{
    private const string LivesKey = "GlobalLives"; // PlayerPrefs-Key für die Leben
    public TMP_Text livesText;  // Textfeld im UI, das die Leben anzeigt

    void Start()
    {
        // Falls noch keine Leben gespeichert wurden, setze sie auf 5
        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, 5);
            PlayerPrefs.Save();
        }

        // Key registrieren für spätere JSON-Exports o. Ä.
        PlayerPrefsKeyTracker.TrackKey(LivesKey);

        // UI sofort aktualisieren
        UpdateLivesUI();
    }

    // Diese Methode kann man manuell aufrufen, um Leben zu ändern und das UI zu aktualisieren
    public void ChangeLives(int newLives)
    {
        PlayerPrefs.SetInt(LivesKey, newLives);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save();
        UpdateLivesUI();
    }

    // Diese Methode aktualisiert den Text im UI
    public void UpdateLivesUI()
    {
        if (livesText != null)
        {
            int currentLives = PlayerPrefs.GetInt(LivesKey, 5); // Hole aktuelle Leben
            livesText.text = currentLives.ToString(); // Zeige sie im Textfeld an
        }
        else
        {
            Debug.LogWarning("⚠️ livesText ist im Inspector nicht zugewiesen!");
        }
    }
}
