using UnityEngine;

public class HealthPowerUp : MonoBehaviour
{
    // Schlüssel für gespeicherte Leben (muss mit PlayerHealthUI übereinstimmen)
    private const string LivesKey = "GlobalLives";

    // Wert, um den die Gesundheit erhöht wird (standardmäßig +1)
    public int healthIncrease = 1;

    // Wird aufgerufen, wenn ein anderer Collider den Trigger betritt.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Überprüfe, ob der Spieler den Trigger betritt.
        if (collision.CompareTag("Player"))
        {
            // Lese die aktuellen Leben aus PlayerPrefs und erhöhe sie.
            int currentLives = PlayerPrefs.GetInt(LivesKey, 5);
            currentLives += healthIncrease;
            PlayerPrefs.SetInt(LivesKey, currentLives);
            PlayerPrefs.Save(); // Speichere die Änderung

            // Suche das PlayerHealthUI-Script und aktualisiere die Anzeige.
            PlayerHealthUI healthUI = FindObjectOfType<PlayerHealthUI>();
            if (healthUI != null)
            {
                healthUI.UpdateLivesUI();
            }
            else
            {
                Debug.LogWarning("Kein PlayerHealthUI in der Szene gefunden!");
            }

            // Zerstöre den Power Up.
            Destroy(gameObject);
        }
    }
}
