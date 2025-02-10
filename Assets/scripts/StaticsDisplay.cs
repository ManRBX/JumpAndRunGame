using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class StaticsDisplay : MonoBehaviour
{
    public TMP_Text outputText;  // TextMeshPro Textfeld für die Statistiken
    private const string LanguageKey = "SelectedLanguage"; // PlayerPrefs Key für die Sprache

    void Start()
    {
        StartCoroutine(InitializeLocalization());
    }

    private IEnumerator InitializeLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        ShowStats(); // Statistiken mit der richtigen Sprache anzeigen
    }

    void ShowStats()
    {
        // Aktuelle Sprache aus PlayerPrefs holen (Standard: Englisch)
        int localeID = PlayerPrefs.GetInt(LanguageKey, 0);
        string languageCode = LocalizationSettings.AvailableLocales.Locales[localeID].Identifier.Code;

        // Strings für die jeweilige Sprache
        string title = (languageCode == "de") ? "SPIEL-STATISTIKEN:\n" : "GAME STATS:\n";
        string kills = (languageCode == "de") ? "Besiegte Gegner: " : "Enemy Kills: ";
        string shots = (languageCode == "de") ? "Abgefeuerte Schüsse: " : "Shots Fired: ";
        string deaths = (languageCode == "de") ? "Tode: " : "Death Count: ";

        // Werte aus den PlayerPrefs auslesen
        string stats = title;
        stats += kills + PlayerPrefs.GetInt("EnemyKills", 0) + "\n";
        stats += shots + PlayerPrefs.GetInt("ShotsFired", 0) + "\n";
        stats += deaths + PlayerPrefs.GetInt("DeathCount", 0) + "\n";

        // Text anzeigen
        if (outputText != null)
        {
            outputText.text = stats;
        }
        else
        {
            Debug.Log(stats);  // Falls kein TMP-Text vorhanden ist, in die Konsole ausgeben
        }
    }
}
