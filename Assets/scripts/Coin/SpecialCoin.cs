using UnityEngine;

public class SpecialCoin : MonoBehaviour
{
    public int coinIndex;           // Index der Münze (für mehrere Münzen im Level)
    public AudioSource collectSoundSource; // AudioSource, die du im Editor zuweisen kannst

    private string coinKey;         // Key zum Speichern, ob die Münze gesammelt wurde
    private string levelKey;        // Key für die speziellen Münzen im Level
    private string globalKey = "GlobalSpecialCoins"; // Globaler Key für alle speziellen Münzen
    private SpriteRenderer spriteRenderer;  // SpriteRenderer für die Transparenz

    void Start()
    {
        // Bestimme den aktuellen Levelnamen
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        coinKey = $"{currentLevel}.Coin{coinIndex}";  // Key für diese Münze im aktuellen Level
        levelKey = $"{currentLevel}-special-coins";   // Key für die Münzen im Level

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Überprüfe, ob die Münze bereits gesammelt wurde, falls ja, setze die Transparenz auf 0
        if (IsCoinCollected())
        {
            SetTransparency(0f);
        }

        if (collectSoundSource != null && collectSoundSource.isPlaying)
        {
            Debug.Log("🛑 Sound spielt ungewollt! Stoppe ihn...");
            collectSoundSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  // Wenn der Spieler die Münze berührt
        {
            if (!IsCoinCollected())  // Nur, wenn die Münze noch nicht eingesammelt wurde
            {
                AddSpecialCoin();  // Füge eine spezielle Münze hinzu

                // Spiele den Sound ab, wenn die Münze eingesammelt wurde
                if (collectSoundSource != null)
                {
                    collectSoundSource.Play();
                }

                // Speichere, dass die Münze jetzt eingesammelt wurde
                PlayerPrefs.SetInt(coinKey, 1);
                PlayerPrefsKeyTracker.TrackKey(coinKey); // Tracke die eingesammelte Münze
                PlayerPrefs.Save();

                // UI aktualisieren
                SpecialCoinUI uiManager = FindObjectOfType<SpecialCoinUI>();
                if (uiManager != null)
                {
                    uiManager.UpdateCoinUI();
                }

                // Setze die Transparenz auf 0, damit die Münze nicht mehr sichtbar ist
                SetTransparency(0f);
            }
        }
    }

    // Fügt eine spezielle Münze hinzu (Punkte und Speicherung)
    void AddSpecialCoin()
    {
        int currentCoins = GetSpecialCoins();
        currentCoins++;

        PlayerPrefs.SetInt(levelKey, currentCoins);  // Speichern der Münzen im Level
        PlayerPrefsKeyTracker.TrackKey(levelKey);  // Tracke die Anzahl der Münzen im Level
        PlayerPrefs.Save();

        AddGlobalSpecialCoin();  // Fügt auch globale Münzen hinzu

        Debug.Log($"Special coin collected! Total special coins for this level: {currentCoins}");
    }

    // Gibt die Anzahl der Münzen im aktuellen Level zurück
    int GetSpecialCoins()
    {
        return PlayerPrefs.GetInt(levelKey, 0);
    }

    // Fügt eine globale Münze hinzu
    void AddGlobalSpecialCoin()
    {
        int globalSpecialCoins = PlayerPrefs.GetInt(globalKey, 0);
        globalSpecialCoins++;

        PlayerPrefs.SetInt(globalKey, globalSpecialCoins);  // Speichern der globalen Münzen
        PlayerPrefsKeyTracker.TrackKey(globalKey);  // Tracke die globalen Münzen
        PlayerPrefs.Save();

        Debug.Log($"Total global special coins: {globalSpecialCoins}");
    }

    // Überprüft, ob die Münze bereits gesammelt wurde
    bool IsCoinCollected()
    {
        return PlayerPrefs.GetInt(coinKey, 0) > 0;
    }

    // Setzt die Transparenz der Münze (um sie unsichtbar zu machen, wenn sie gesammelt wurde)
    private void SetTransparency(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}
