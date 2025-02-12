using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class StaticsDisplay : MonoBehaviour
{
    public TMP_Text outputText;  // TextMeshPro text field for displaying statistics
    private const string LanguageKey = "SelectedLanguage"; // PlayerPrefs key for the language

    void Start()
    {
        StartCoroutine(InitializeLocalization());
    }

    private IEnumerator InitializeLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        ShowStats(); // Display statistics in the correct language
    }

    void ShowStats()
    {
        // Get the current language from PlayerPrefs (default: English)
        int localeID = PlayerPrefs.GetInt(LanguageKey, 0);
        string languageCode = LocalizationSettings.AvailableLocales.Locales[localeID].Identifier.Code;

        // Strings for each language
        string title = (languageCode == "de") ? "GAME STATISTICS:\n" : "GAME STATS:\n";
        string kills = (languageCode == "de") ? "Enemies Defeated: " : "Enemy Kills: ";
        string shots = (languageCode == "de") ? "Shots Fired: " : "Shots Fired: ";
        string deaths = (languageCode == "de") ? "Deaths: " : "Death Count: ";

        // Retrieve values from PlayerPrefs
        string stats = title;
        stats += kills + PlayerPrefs.GetInt("EnemyKills", 0) + "\n";
        stats += shots + PlayerPrefs.GetInt("ShotsFired", 0) + "\n";
        stats += deaths + PlayerPrefs.GetInt("DeathCount", 0) + "\n";

        // Display the text
        if (outputText != null)
        {
            outputText.text = stats;
        }
        else
        {
            Debug.Log(stats);  // If no TMP text is assigned, output to console
        }
    }
}
