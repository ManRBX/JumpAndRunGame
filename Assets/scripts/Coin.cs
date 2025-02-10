using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Einstellungen")]
    public int pointValue = 100;  // Punktewert der Münze
    public int coinValue = 1;     // Anzahl der Münzen, die der Spieler erhält
    public AudioClip collectSound;

    private AudioSource audioSource;

    private void Start()
    {
        // Audio-Komponente initialisieren
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = collectSound;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Punkte und Coins vergeben
            AddPointsAndCoinsToPlayer();

            Debug.Log($"Münze eingesammelt. Punkte: {pointValue}, Coins: {coinValue}");

            // Sound abspielen
            PlayCollectSound();

            // Münze zerstören
            Destroy(gameObject, 0.1f);
        }
    }

    private void AddPointsAndCoinsToPlayer()
    {
        // Punkte erhöhen
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddPoints(pointValue);
            CoinManager.Instance.AddCoins(coinValue);
        }
    }

    private void PlayCollectSound()
    {
        if (audioSource != null && collectSound != null)
        {
            audioSource.Play();
        }
    }
}
