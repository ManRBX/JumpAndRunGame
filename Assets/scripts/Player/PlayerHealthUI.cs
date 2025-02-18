using UnityEngine;
using TMPro;  // TMP import

public class PlayerHealthUI : MonoBehaviour
{
    private const string LivesKey = "GlobalLives"; // Key for stored lives
    public TMP_Text livesText;  // TMP_Text for the lives display

    void Start()
    {
        UpdateLivesUI(); // Update UI on start
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
        {
            int currentLives = PlayerPrefs.GetInt(LivesKey, 3); // Retrieve lives from PlayerPrefs (default: 3)
            livesText.text = currentLives.ToString(); // Display only the number
        }
        else
        {
            Debug.LogWarning("livesText is not assigned!");
        }
    }
}
