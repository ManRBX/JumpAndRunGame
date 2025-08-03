using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    public int pointValue = 100;  // Point value of the coin
    public int coinValue = 1;     // Number of coins the player receives
    public AudioClip collectSound;

    private AudioSource audioSource;

    private void Start()
    {
        // Initialize audio component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = collectSound;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Award points and coins
            AddPointsAndCoinsToPlayer();

            Debug.Log($"Coin collected. Points: {pointValue}, Coins: {coinValue}");

            // Play sound effect
            PlayCollectSound();

            // Destroy the coin
            Destroy(gameObject, 0.1f);
        }
    }

    private void AddPointsAndCoinsToPlayer()
    {
        // Increase points
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
