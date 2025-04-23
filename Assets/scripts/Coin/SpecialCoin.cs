using UnityEngine;

public class SpecialCoin : MonoBehaviour
{
    public int coinIndex;
    public AudioClip collectSound;

    private AudioSource audioSource;
    private string coinKey;
    private string levelKey;
    private string globalKey = "GlobalSpecialCoins";
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        coinKey = $"{currentLevel}.Coin{coinIndex}";
        levelKey = $"{currentLevel}-special-coins";

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (IsCoinCollected())
        {
            SetTransparency(0f);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = collectSound;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!IsCoinCollected())
            {
                AddSpecialCoin();

                if (collectSound != null)
                {
                    audioSource.Play();
                }

                PlayerPrefs.SetInt(coinKey, 1);
                PlayerPrefsKeyTracker.TrackKey(coinKey); // 🟢 Track collected coin
                PlayerPrefs.Save();

                SpecialCoinUI uiManager = FindFirstObjectByType<SpecialCoinUI>();
                if (uiManager != null)
                {
                    uiManager.UpdateCoinUI();
                }

                SetTransparency(0f);
            }
        }
    }

    void AddSpecialCoin()
    {
        int currentCoins = GetSpecialCoins();
        currentCoins++;

        PlayerPrefs.SetInt(levelKey, currentCoins);
        PlayerPrefsKeyTracker.TrackKey(levelKey); // 🟢 Track level-specific coin count
        PlayerPrefs.Save();

        AddGlobalSpecialCoin();

        Debug.Log($"Special coin collected! Total special coins for this level: {currentCoins}");
    }

    int GetSpecialCoins()
    {
        return PlayerPrefs.GetInt(levelKey, 0);
    }

    void AddGlobalSpecialCoin()
    {
        int globalSpecialCoins = PlayerPrefs.GetInt(globalKey, 0);
        globalSpecialCoins++;

        PlayerPrefs.SetInt(globalKey, globalSpecialCoins);
        PlayerPrefsKeyTracker.TrackKey(globalKey); // 🟢 Track global special coin count
        PlayerPrefs.Save();

        Debug.Log($"Total global special coins: {globalSpecialCoins}");
    }

    bool IsCoinCollected()
    {
        return PlayerPrefs.GetInt(coinKey, 0) > 0;
    }

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
