using UnityEngine;

public class Starter : MonoBehaviour
{
    [Header("Starter Einstellungen")]
    [Tooltip("Anzahl der Collectible Items, die in diesem MiniGame eingesammelt werden müssen.")]
    public int numberOfCollectibles = 3;

    [Tooltip("MiniGame-Dauer in Sekunden.")]
    public float miniGameDuration = 10f;

    [Tooltip("Sammel-Objekte, die beim Starter sichtbar werden sollen.")]
    public GameObject[] collectibleItems;

    [Tooltip("Prefab für die Münzen, in die die Collectibles verwandelt werden sollen.")]
    public GameObject coinPrefab;

    private bool isActivated = false;
    private bool rewardGiven = false; // Verhindert mehrfaches Belohnen

    private void Start()
    {
        // Stelle sicher, dass die Collectible Items zu Beginn deaktiviert sind.
        foreach (var item in collectibleItems)
        {
            if (item != null)
                item.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated)
            return;

        if (other.CompareTag("Player"))
        {
            ActivateStarter();
        }
    }

    private void ActivateStarter()
    {
        isActivated = true;
        Debug.Log("Starter aktiviert – MiniGame startet!");

        if (MiniGameManager.Instance != null)
        {
            // MiniGame starten und Callback setzen
            MiniGameManager.Instance.StartMiniGame(numberOfCollectibles, miniGameDuration, OnMiniGameComplete, OnMiniGameFailed);
        }

        // Alle Collectibles sichtbar machen
        foreach (var item in collectibleItems)
        {
            if (item != null)
                item.SetActive(true);
        }

        // Deaktiviere den Starter selbst
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Wenn der Spieler ALLE Collectibles innerhalb der Zeit einsammelt
    /// </summary>
    private void OnMiniGameComplete()
    {
        if (rewardGiven) return;
        rewardGiven = true;

        Debug.Log("✅ MiniGame erfolgreich abgeschlossen! +1 Leben erhalten!");

        // +1 Leben geben
        int currentLives = PlayerPrefs.GetInt("GlobalLives", 3);
        currentLives++;
        PlayerPrefs.SetInt("GlobalLives", currentLives);
        PlayerPrefs.Save();

        // Falls eine UI existiert, aktualisieren
        PlayerHealthUI healthUI = FindObjectOfType<PlayerHealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateLebenUI();
        }
    }

    /// <summary>
    /// Wenn das MiniGame fehlschlägt (Zeit abgelaufen)
    /// </summary>
    private void OnMiniGameFailed()
    {
        Debug.Log("❌ MiniGame gescheitert – Collectibles werden zu Coins!");

        // Alle übrig gebliebenen Collectibles in Coins umwandeln
        foreach (var item in collectibleItems)
        {
            if (item != null && item.activeSelf) // Nur aktive Items umwandeln
            {
                Vector3 spawnPosition = item.transform.position;
                Destroy(item); // Entferne das Collectible-Item
                Instantiate(coinPrefab, spawnPosition, Quaternion.identity); // Ersetze es durch eine Münze
            }
        }
    }
}
