using UnityEngine;
using UnityEngine.Audio;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    public int pointValue = 100;  // Punktwert der Münze
    public int coinValue = 1;     // Anzahl der Münzen, die der Spieler erhält
    public AudioSource collectSound;  // AudioSource für den Sound, wenn die Münze eingesammelt wird

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Punkte und Münzen dem Spieler hinzufügen
            AddPointsAndCoinsToPlayer();

            Debug.Log($"Münze eingesammelt. Punkte: {pointValue}, Münzen: {coinValue}");

            // Soundeffekt abspielen
            PlayCollectSound();

            // Zerstöre die Münze nach kurzer Zeit
            Destroy(gameObject, 0.1f);
        }
    }

    private void AddPointsAndCoinsToPlayer()
    {
        // Punkte und Münzen erhöhen
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddPoints(pointValue);
            CoinManager.Instance.AddCoins(coinValue);
        }
    }

    private void PlayCollectSound()
    {
        // Stelle sicher, dass der collectSound vorhanden ist, bevor er abgespielt wird
        if (collectSound != null)
        {
            collectSound.Play();
        }
    }
}
