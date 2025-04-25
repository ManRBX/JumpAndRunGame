using UnityEngine;
using TMPro;  // TMP import

public class PlayerHealthUI : MonoBehaviour
{
    private const string LivesKey = "GlobalLives"; // Key for stored lives
    public TMP_Text livesText;  // TMP_Text for the lives display

    void Start()
    {
        if (!PlayerPrefs.HasKey(LivesKey))
        {
            PlayerPrefs.SetInt(LivesKey, 5);
            PlayerPrefs.Save();
        }

        PlayerPrefsKeyTracker.TrackKey(LivesKey); // 👈 immer tracken, auch wenn er schon da ist
        UpdateLivesUI();
    }

    public void ChangeLives(int newLives)
    {
        PlayerPrefs.SetInt(LivesKey, newLives);
        PlayerPrefsKeyTracker.TrackKey(LivesKey);
        PlayerPrefs.Save();
        UpdateLivesUI();
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
        {
            int currentLives = PlayerPrefs.GetInt(LivesKey, 5); // Hole die Leben aus den PlayerPrefs (Standardwert: 5)
            livesText.text = currentLives.ToString(); // Setze die Textanzeige auf die aktuelle Anzahl der Leben
        }
        else
        {
            Debug.LogWarning("livesText ist nicht zugewiesen!");
        }
    }
}
