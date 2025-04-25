using UnityEngine;

public class HealthPowerUp : MonoBehaviour
{
    private const string LivesKey = "GlobalLives";
    public int healthIncrease = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int currentLives = PlayerPrefs.GetInt(LivesKey, 5);
            currentLives += healthIncrease;

            PlayerPrefs.SetInt(LivesKey, currentLives);
            PlayerPrefsKeyTracker.TrackKey(LivesKey);
            PlayerPrefs.Save();

            PlayerHealthUI healthUI = FindFirstObjectByType<PlayerHealthUI>();
            if (healthUI != null)
            {
                healthUI.UpdateLivesUI();
            }
            else
            {
                Debug.LogWarning("Kein PlayerHealthUI in der Szene gefunden!");
            }

            Destroy(gameObject); // Power-Up entfernen
        }
    }
}
