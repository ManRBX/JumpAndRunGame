using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private const string LanguageKey = "SelectedLanguage";
    private bool isSwitching = false; // Prevents switching multiple times

    private void Start()
    {
        int savedLocaleID = PlayerPrefs.GetInt(LanguageKey, 0);
        ChangeLocale(savedLocaleID);
    }

    /// <summary>
    /// Switches the language based on the passed locale ID.
    /// </summary>
    public void ChangeLocale(int localeID)
    {
        if (isSwitching) return; // If a switch is in progress, abort
        StartCoroutine(SetLocale(localeID));
    }

    /// <summary>
    /// Sets the language and saves it in PlayerPrefs.
    /// </summary>
    private IEnumerator SetLocale(int localeID)
    {
        isSwitching = true;
        yield return LocalizationSettings.InitializationOperation; // Wait until the localization system is ready

        if (localeID < 0 || localeID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            Debug.LogError($"Invalid Locale ID: {localeID}. Setting to default value (0).");
            localeID = 0;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt(LanguageKey, localeID);
        PlayerPrefs.Save();

        Debug.Log($"🌍 Language changed to: {LocalizationSettings.SelectedLocale.LocaleName}");

        isSwitching = false;
    }
}
