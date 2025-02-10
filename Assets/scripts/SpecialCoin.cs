using UnityEngine;

public class SpecialCoin : MonoBehaviour
{
    public int coinIndex;  // Index der Münze (z. B. 0, 1, 2)
    public AudioClip collectSound;  // Sound beim Einsammeln

    private AudioSource audioSource;
    private string coinKey;  // Key für diese spezielle Münze im PlayerPrefs
    private SpriteRenderer spriteRenderer;  // Renderer der Münze

    void Start()
    {
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        coinKey = $"{currentLevel}.Coin{coinIndex}";  // Einzigartiger Key für diese Münze

        // SpriteRenderer referenzieren
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Prüfen, ob die Münze schon eingesammelt wurde
        if (PlayerPrefs.HasKey(coinKey))
        {
            // Münze transparent machen
            SetTransparency(0.5f);
        }

        // AudioSource hinzufügen
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = collectSound;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Spezialmünzen nur einsammeln, wenn sie noch nicht eingesammelt wurde
            if (!PlayerPrefs.HasKey(coinKey))
            {
                AddSpecialCoin();

                // Sound abspielen
                if (collectSound != null)
                {
                    audioSource.Play();
                }

                // Münze im PlayerPrefs speichern
                PlayerPrefs.SetInt(coinKey, 1);
                PlayerPrefs.Save();

                // UI aktualisieren
                SpecialCoinUI uiManager = FindObjectOfType<SpecialCoinUI>();
                if (uiManager != null)
                {
                    uiManager.CollectCoin(coinIndex);  // Zeige die Münze in der UI
                }

                // Münze transparent machen
                SetTransparency(0.1f);
            }
        }
    }

    void AddSpecialCoin()
    {
        const string globalKey = "GlobalSpecialCoins";  // Key für globale Spezialmünzen
        int globalSpecialCoins = PlayerPrefs.GetInt(globalKey, 0);
        globalSpecialCoins += 1;
        PlayerPrefs.SetInt(globalKey, globalSpecialCoins);

        Debug.Log($"Spezialmünze eingesammelt! Globale Spezialmünzen: {globalSpecialCoins}");
    }

    // Sichtbarkeit der Münze ändern
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
