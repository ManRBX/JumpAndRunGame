using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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

    private static Dictionary<int, float> fragmentCooldownTimers = new Dictionary<int, float>();
    private static bool isQuitting = false;
    private bool isBroken = false;
    public bool startInvisible = false;

    void Start()
    {
        currentHitPoints = hitPoints;

        if (startInvisible)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = false;
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
        if (isBroken)
            return;

        currentHitPoints--;

        if (currentHitPoints <= 0)
        {
            isBroken = true;
            BreakBox();
        }
    }

    void BreakBox()
    {
        if (!isBroken)
            isBroken = true;

        if (fragmentsPrefabs != null && fragmentsPrefabs.Length > 0)
        {
            if (Random.value <= randomSettings.fragmentSpawnChance)
            {
                List<int> availableIndices = new List<int>();
                float currentTime = Time.time;

                for (int i = 0; i < fragmentsPrefabs.Length; i++)
                {
                    if (!fragmentCooldownTimers.ContainsKey(i) || (currentTime - fragmentCooldownTimers[i]) >= fragmentCooldown)
                        availableIndices.Add(i);
                }

                if (availableIndices.Count > 0)
                {
                    int randomAvailableIndex = availableIndices[Random.Range(0, availableIndices.Count)];
                    fragmentCooldownTimers[randomAvailableIndex] = currentTime;

                    GameObject selectedPrefab = fragmentsPrefabs[randomAvailableIndex];
                    Vector3 spawnPos = transform.position + new Vector3(0, fragmentSpawnYOffset, 0);
                    GameObject fragments = Instantiate(selectedPrefab, spawnPos, transform.rotation);

                    Transform[] allChildren = fragments.GetComponentsInChildren<Transform>(true);
                    List<GameObject> childObjects = new List<GameObject>();

                    for (int c = 1; c < allChildren.Length; c++)
                        childObjects.Add(allChildren[c].gameObject);

                    foreach (GameObject child in childObjects)
                        child.SetActive(false);

                    if (childObjects.Count > 0)
                    {
                        int randomChildIndex = Random.Range(0, childObjects.Count);
                        childObjects[randomChildIndex].SetActive(true);
                    }

                    foreach (Rigidbody2D rb in fragments.GetComponentsInChildren<Rigidbody2D>())
                    {
                        Vector2 direction = (rb.transform.position - transform.position).normalized;
                        rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && brokenSprite != null)
            spriteRenderer.sprite = brokenSprite;

        string currentLevel = SceneManager.GetActiveScene().name;

        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        globalPoints += awardedPoints;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);
        PlayerPrefsKeyTracker.TrackKey("GlobalPoints");

        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        levelPoints += awardedPoints;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);
        PlayerPrefsKeyTracker.TrackKey($"{currentLevel}_Points");

        if (Random.value <= randomSettings.coinAwardChance)
        {
            int globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
            globalCoins += randomSettings.awardedCoins;
            PlayerPrefs.SetInt("GlobalCoins", globalCoins);
            PlayerPrefsKeyTracker.TrackKey("GlobalCoins");

            int levelCoins = PlayerPrefs.GetInt($"{currentLevel}_Coins", 0);
            levelCoins += randomSettings.awardedCoins;
            PlayerPrefs.SetInt($"{currentLevel}_Coins", levelCoins);
            PlayerPrefsKeyTracker.TrackKey($"{currentLevel}_Coins");
        }

        PlayerPrefs.Save();

        CoinStatsDisplay statsDisplay = FindFirstObjectByType<CoinStatsDisplay>();
        if (statsDisplay != null)
        {
            statsDisplay.UpdatePointStats();
        }

        if (destroyOnBreak)
        {
            PlayDestructionEffect();
            Destroy(gameObject);
        }
    }

    void PlayDestructionEffect()
    {
        if (isQuitting)
            return;

        if (destructionEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(effectInstance, ps.main.duration);
            else
                Destroy(effectInstance, 2f);
        }
    }
}
