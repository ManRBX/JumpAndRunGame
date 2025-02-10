using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private const string LanguageKey = "SelectedLanguage";
    private bool isSwitching = false; // Verhindert mehrfaches Umschalten

    private void Start()
    {
        int savedLocaleID = PlayerPrefs.GetInt(LanguageKey, 0);
        ChangeLocale(savedLocaleID);
    }

    /// <summary>
    /// Wechselt die Sprache anhand der übergebenen ID.
    /// </summary>
    public void ChangeLocale(int localeID)
    {
        if (isSwitching) return; // Falls gerade eine Umstellung läuft, abbrechen
        StartCoroutine(SetLocale(localeID));
    }

    /// <summary>
    /// Stellt die Sprache ein und speichert sie in den PlayerPrefs.
    /// </summary>
    private IEnumerator SetLocale(int localeID)
    {
        isSwitching = true;
        yield return LocalizationSettings.InitializationOperation; // Warten, bis das Localization-System bereit ist

        if (localeID < 0 || localeID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            Debug.LogError($"Ungültige Locale-ID: {localeID}. Setze Standardwert (0).");
            localeID = 0;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt(LanguageKey, localeID);
        PlayerPrefs.Save();

        Debug.Log($"🌍 Sprache gewechselt zu: {LocalizationSettings.SelectedLocale.LocaleName}");

        isSwitching = false;
    }
}
