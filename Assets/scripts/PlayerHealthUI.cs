using UnityEngine;
using TMPro;  // TMP-Import

public class PlayerHealthUI : MonoBehaviour
{
    private const string LivesKey = "GlobalLives"; // Key für gespeicherte Leben
    public TMP_Text lebenText;  // TMP_Text für die Lebensanzeige

    void Start()
    {
        UpdateLebenUI(); // UI beim Start aktualisieren
    }

    public void UpdateLebenUI()
    {
        if (lebenText != null)
        {
            int currentLives = PlayerPrefs.GetInt(LivesKey, 3); // Leben aus PlayerPrefs holen (Standard: 3)
            lebenText.text = currentLives.ToString(); // Zeigt nur die Zahl an
        }
        else
        {
            Debug.LogWarning("lebenText ist nicht zugewiesen!");
        }
    }
}
