using UnityEngine;

public class PlayerPrefsInitializer : MonoBehaviour
{
    private void Start()
    {
        if (ResetSession.wasReset)
        {
            Debug.Log("🚨 Frischer Reset – Initialwerte werden gesetzt...");

            PlayerPrefs.SetInt("GlobalPoints", 0);
            PlayerPrefsKeyTracker.TrackKey("GlobalPoints");

            PlayerPrefs.SetInt("GlobalCoins", 0);
            PlayerPrefsKeyTracker.TrackKey("GlobalCoins");

            PlayerPrefs.SetInt("GlobalLives", 5);
            PlayerPrefsKeyTracker.TrackKey("GlobalLives");

            PlayerPrefs.SetInt("EnemyKills", 0);
            PlayerPrefsKeyTracker.TrackKey("EnemyKills");

            PlayerPrefs.SetFloat("Volume", 0.5f);
            PlayerPrefsKeyTracker.TrackKey("Volume");

            PlayerPrefs.SetInt("Fullscreen", 1);
            PlayerPrefsKeyTracker.TrackKey("Fullscreen");

            PlayerPrefs.SetInt("SelectedLanguage", 0);
            PlayerPrefsKeyTracker.TrackKey("SelectedLanguage");

            PlayerPrefs.SetInt("ShotsFired", 0);
            PlayerPrefsKeyTracker.TrackKey("ShotsFired");

            PlayerPrefs.SetInt("GlobalAmmo", 60);
            PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");

            PlayerPrefs.SetInt("Level01_Unlocked", 1);
            PlayerPrefsKeyTracker.TrackKey("Level01_Unlocked");

            PlayerPrefs.SetInt("Level01_Visited", 1);
            PlayerPrefsKeyTracker.TrackKey("Level01_Visited");

            PlayerPrefs.Save();

            Debug.Log("✅ Initialwerte gesetzt.");

            ResetSession.wasReset = false;
        }
    }
}
