using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BreakableBox2D : MonoBehaviour
{
    public int hitPoints = 1;
    private int currentHitPoints;

    public GameObject[] fragmentsPrefabs;
    public float explosionForce = 300f;
    public float explosionRadius = 2f;
    public GameObject destructionEffectPrefab;
    public Sprite brokenSprite;
    public bool destroyOnBreak = false;
    public int awardedPoints = 10;
    public RandomSettings randomSettings;
    [SerializeField] private float fragmentSpawnYOffset = 1f;
    [SerializeField] private float fragmentCooldown = 20f;

    [Header("\ud83d\udcca Zufallsverhalten")]
    public BoxBehaviorChances behaviorChances = new BoxBehaviorChances();

    [Header("\ud83c\udff1 PowerUp Drops")]
    [Range(0f, 1f)] public float globalPowerUpChance = 0.4f;
    public List<PowerUpDrop> possiblePowerUps = new List<PowerUpDrop>();
    [SerializeField] private float powerUpCooldownSeconds = 30f;

    [Header("\ud83d\udcb0 Coin Drop")]
    public GameObject coinPrefab;
    public float coinSpawnYOffset = 1.2f;

    private static Dictionary<int, float> fragmentCooldownTimers = new Dictionary<int, float>();
    private static float lastDropTime = -9999f;
    private static GameObject lastSpawnedDrop = null;
    private static bool isQuitting = false;
    private bool isBroken = false;

    [Header("🔊 destruction-sound")]
    public AudioSource destructionSound;

    [Header("🔊 hit-sound")]
    public AudioSource hitSound;

    void Start()
    {
        currentHitPoints = hitPoints;

        if (Random.value <= behaviorChances.startInvisibleChance)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
        }
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && !sr.enabled)
                sr.enabled = true;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    ApplyHit();
                    break;
                }
            }
        }
    }

    void ApplyHit()
    {
        if (isBroken) return;

        if (hitSound != null && !hitSound.isPlaying)
            hitSound.Play();

        currentHitPoints--;

        if (currentHitPoints <= 0)
        {
            isBroken = true;
            BreakBox();
        }
    }

    void BreakBox()
    {
        if (!isBroken) isBroken = true;

        TrySpawnFragments();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && brokenSprite != null)
            sr.sprite = brokenSprite;

        string currentLevel = SceneManager.GetActiveScene().name;

        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0) + awardedPoints;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);
        PlayerPrefsKeyTracker.TrackKey("GlobalPoints");

        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0) + awardedPoints;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);
        PlayerPrefsKeyTracker.TrackKey($"{currentLevel}_Points");

        TryDropReward();
        PlayerPrefs.Save();

        CoinStatsDisplay statsDisplay = FindFirstObjectByType<CoinStatsDisplay>();
        if (statsDisplay != null)
            statsDisplay.UpdatePointStats();

        if (destructionSound != null && !destructionSound.isPlaying)
            destructionSound.Play();

        if (Random.value <= behaviorChances.shouldBreakChance)
        {
            if (Random.value <= behaviorChances.showDestructionEffectChance)
                PlayDestructionEffect();

            if (destroyOnBreak)
                Destroy(gameObject);
        }
    }


    void TrySpawnFragments()
    {
        if (fragmentsPrefabs == null || fragmentsPrefabs.Length == 0) return;
        if (Random.value > randomSettings.fragmentSpawnChance) return;

        float currentTime = Time.time;
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < fragmentsPrefabs.Length; i++)
        {
            if (!fragmentCooldownTimers.ContainsKey(i) || (currentTime - fragmentCooldownTimers[i]) >= fragmentCooldown)
                availableIndices.Add(i);
        }

        if (availableIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            fragmentCooldownTimers[randomIndex] = currentTime;

            GameObject prefab = fragmentsPrefabs[randomIndex];
            Vector3 spawnPos = transform.position + new Vector3(0, fragmentSpawnYOffset, 0);
            GameObject fragments = Instantiate(prefab, spawnPos, transform.rotation);

            Transform[] allChildren = fragments.GetComponentsInChildren<Transform>(true);
            List<GameObject> childObjects = new List<GameObject>();

            for (int c = 1; c < allChildren.Length; c++)
                childObjects.Add(allChildren[c].gameObject);

            foreach (GameObject child in childObjects)
                child.SetActive(false);

            if (childObjects.Count > 0)
                childObjects[Random.Range(0, childObjects.Count)].SetActive(true);

            foreach (Rigidbody2D rb in fragments.GetComponentsInChildren<Rigidbody2D>())
            {
                Vector2 dir = (rb.transform.position - transform.position).normalized;
                rb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }
    }

    void TryDropReward()
    {
        if (Time.time - lastDropTime < powerUpCooldownSeconds)
            return;

        bool dropPowerUp = Random.value <= globalPowerUpChance && possiblePowerUps.Count > 0;
        bool dropCoin = Random.value <= randomSettings.coinAwardChance && coinPrefab != null;

        if (!dropPowerUp && !dropCoin)
            return;

        bool givePowerUp = dropPowerUp && (!dropCoin || Random.value > 0.5f);

        if (givePowerUp)
        {
            List<GameObject> validDrops = new List<GameObject>();
            foreach (PowerUpDrop drop in possiblePowerUps)
            {
                if (drop.powerUpPrefab != null && Random.value <= drop.individualChance && drop.powerUpPrefab != lastSpawnedDrop)
                    validDrops.Add(drop.powerUpPrefab);
            }

            if (validDrops.Count > 0)
            {
                GameObject selected = validDrops[Random.Range(0, validDrops.Count)];
                lastDropTime = Time.time;
                lastSpawnedDrop = selected;

                Vector3 spawnPos = transform.position + new Vector3(0f, 1.2f, 0f);
                Instantiate(selected, spawnPos, Quaternion.identity);

                string itemName = selected.name;
                int currentAmount = PlayerPrefs.GetInt($"PowerUp_{itemName}", 0) + 1;
                PlayerPrefs.SetInt($"PowerUp_{itemName}", currentAmount);
                PlayerPrefsKeyTracker.TrackKey($"PowerUp_{itemName}");
            }
        }
        else
        {
            lastDropTime = Time.time;

            Vector3 spawnPos = transform.position + new Vector3(0f, coinSpawnYOffset, 0f);
            GameObject coinObj = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            Coin pickup = coinObj.GetComponent<Coin>();
            if (pickup != null)
                pickup.coinValue = randomSettings.awardedCoins;

            string currentLevel = SceneManager.GetActiveScene().name;

            int globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0) + randomSettings.awardedCoins;
            PlayerPrefs.SetInt("GlobalCoins", globalCoins);
            PlayerPrefsKeyTracker.TrackKey("GlobalCoins");

            int levelCoins = PlayerPrefs.GetInt($"{currentLevel}_Coins", 0) + randomSettings.awardedCoins;
            PlayerPrefs.SetInt($"{currentLevel}_Coins", levelCoins);
            PlayerPrefsKeyTracker.TrackKey($"{currentLevel}_Coins");
        }
    }

    void PlayDestructionEffect()
    {
        if (isQuitting || destructionEffectPrefab == null) return;

        GameObject fx = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        Destroy(fx, ps ? ps.main.duration : 2f);
    }
}
