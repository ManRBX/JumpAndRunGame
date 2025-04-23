using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private const string LanguageKey = "SelectedLanguage";
    private bool isSwitching = false;

    private void Start()
    {
        int savedLocaleID = PlayerPrefs.GetInt(LanguageKey, 0);
        PlayerPrefsKeyTracker.TrackKey(LanguageKey); // ✅ Auch beim Laden tracken
        ChangeLocale(savedLocaleID);
    }

    /// <summary>
    /// Gibt zurück, ob gerade ein Sprachwechsel läuft (z. B. für UI-Sperre).
    /// </summary>
    public bool IsBusy() => isSwitching;

    /// <summary>
    /// Wechselt die Sprache über die Locale-ID.
    /// </summary>
    public void ChangeLocale(int localeID)
    {
        if (isSwitching) return;
        StartCoroutine(SetLocale(localeID));
    }

    /// <summary>
    /// Wechselt die Sprache über einen Sprachcode, z. B. "en", "de", "fr".
    /// </summary>
    public void ChangeLocaleByCode(string languageCode)
    {
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == languageCode)
            {
                ChangeLocale(i);
                return;
            }
        }

        Debug.LogWarning($"🌐 Language code '{languageCode}' not found in available locales.");
    }

    /// <summary>
    /// Setzt die Sprache und speichert sie in PlayerPrefs.
    /// </summary>
    private IEnumerator SetLocale(int localeID)
    {
        isSwitching = true;

        yield return LocalizationSettings.InitializationOperation;

        if (localeID < 0 || localeID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            Debug.LogError($"❌ Invalid Locale ID: {localeID}. Using default (0).");
            localeID = 0;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt(LanguageKey, localeID);
        PlayerPrefsKeyTracker.TrackKey(LanguageKey);
        PlayerPrefs.Save();

        Debug.Log($"🌍 Language changed to: {LocalizationSettings.SelectedLocale.LocaleName}");

        isSwitching = false;
    }
}
