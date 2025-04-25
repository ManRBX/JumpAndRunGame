using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using System.Collections;

public class StaticsDisplay : MonoBehaviour
{
    public TMP_Text outputText;

    private const string LanguageKey = "SelectedLanguage";

    void Start()
    {
        StartCoroutine(InitializeLocalization());
    }

    private IEnumerator InitializeLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        ShowStats();
    }

    void ShowStats()
    {
        int localeID = PlayerPrefs.GetInt(LanguageKey, 0);
        PlayerPrefsKeyTracker.TrackKey(LanguageKey);

        string languageCode = LocalizationSettings.AvailableLocales.Locales[localeID].Identifier.Code;

        string title, kills, shots, deaths;

        switch (languageCode)
        {
            case "de":
                title = "SPIELSTATISTIK:\n";
                kills = "Besiegte Gegner: ";
                shots = "Abgefeuerte Schüsse: ";
                deaths = "Tode: ";
                break;

            default:
                title = "GAME STATS:\n";
                kills = "Enemy Kills: ";
                shots = "Shots Fired: ";
                deaths = "Death Count: ";
                break;
        }

        int killCount = PlayerPrefs.GetInt("EnemyKills", 0);
        int shotCount = PlayerPrefs.GetInt("ShotsFired", 0);
        int deathCount = PlayerPrefs.GetInt("DeathCount", 0);

        PlayerPrefsKeyTracker.TrackKey("EnemyKills");
        PlayerPrefsKeyTracker.TrackKey("ShotsFired");
        PlayerPrefsKeyTracker.TrackKey("DeathCount");

        string stats = $"{title}{kills}{killCount}\n{shots}{shotCount}\n{deaths}{deathCount}";

        if (outputText != null)
        {
            outputText.text = stats;
        }
        else
        {
            Debug.Log(stats);
        }
    }
}
