using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    [Tooltip("Eindeutiger Index der Münze innerhalb des Levels.")]
    public int coinIndex;

    [Header("Sound")]
    [Tooltip("AudioSource für den Einsammel-Sound.")]
    public AudioSource collectSoundSource;

    private string coinKey;
    private string levelKey;

    private const string GlobalKey = "GlobalSpecialCoins";

    private SpriteRenderer[] spriteRenderers;
    private Collider2D[] coinColliders;

    private bool collected = false;

    private void Start()
    {
        string currentLevel = SceneManager.GetActiveScene().name;

        coinKey = $"{currentLevel}.Coin{coinIndex}";
        levelKey = $"{currentLevel}-special-coins";

        // Alle SpriteRenderer finden, auch in Child-Objekten
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // Alle Collider finden
        coinColliders = GetComponentsInChildren<Collider2D>(true);

        if (collectSoundSource != null && collectSoundSource.isPlaying)
        {
            collectSoundSource.Stop();
        }

        // Bereits früher eingesammelt?
        if (IsCoinCollected())
        {
            collected = true;

            SetCoinTransparency(0f);
            DisableCoinColliders();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)
            return;

        if (!collision.CompareTag("Player"))
            return;

        collected = true;

        // Coins zählen
        AddSpecialCoin();

        // Einzelnen Coin speichern
        PlayerPrefs.SetInt(coinKey, 1);
        PlayerPrefsKeyTracker.TrackKey(coinKey);

        PlayerPrefs.Save();

        // UI aktualisieren
        SpecialCoinUI uiManager =
            FindFirstObjectByType<SpecialCoinUI>();

        if (uiManager != null)
        {
            uiManager.UpdateCoinUI();
        }

        // Sound abspielen
        if (collectSoundSource != null)
        {
            collectSoundSource.Play();
        }

        // ==================================================
        // COIN SOFORT 100 % TRANSPARENT MACHEN
        // ==================================================

        SetCoinTransparency(0f);

        // Collider deaktivieren
        DisableCoinColliders();

        Debug.Log(
            $"⭐ Special Coin {coinIndex} eingesammelt! " +
            $"Coin wurde vollständig transparent gemacht."
        );
    }

    private void AddSpecialCoin()
    {
        int currentCoins = GetSpecialCoins();

        currentCoins++;

        PlayerPrefs.SetInt(
            levelKey,
            currentCoins
        );

        PlayerPrefsKeyTracker.TrackKey(levelKey);

        AddGlobalSpecialCoin();

        PlayerPrefs.Save();

        Debug.Log(
            $"⭐ Special Coin gesammelt. " +
            $"Level Coins: {currentCoins} | " +
            $"Key: {levelKey}"
        );
    }

    private int GetSpecialCoins()
    {
        return PlayerPrefs.GetInt(
            levelKey,
            0
        );
    }

    private void AddGlobalSpecialCoin()
    {
        int globalSpecialCoins =
            PlayerPrefs.GetInt(
                GlobalKey,
                0
            );

        globalSpecialCoins++;

        PlayerPrefs.SetInt(
            GlobalKey,
            globalSpecialCoins
        );

        PlayerPrefsKeyTracker.TrackKey(
            GlobalKey
        );

        Debug.Log(
            $"🌍 Globale Special Coins: " +
            $"{globalSpecialCoins}"
        );
    }

    private bool IsCoinCollected()
    {
        return PlayerPrefs.GetInt(
            coinKey,
            0
        ) > 0;
    }

    private void SetCoinTransparency(float alpha)
    {
        if (spriteRenderers == null)
            return;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == null)
                continue;

            Color currentColor =
                spriteRenderer.color;

            currentColor.a = alpha;

            spriteRenderer.color =
                currentColor;
        }
    }

    private void DisableCoinColliders()
    {
        if (coinColliders == null)
            return;

        foreach (Collider2D coinCollider in coinColliders)
        {
            if (coinCollider == null)
                continue;

            coinCollider.enabled = false;
        }
    }
}