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

    [Header("📊 Zufallsverhalten")]
    public BoxBehaviorChances behaviorChances = new BoxBehaviorChances();

    [Header("🏁 Drop-Modus")]
    [Tooltip("Wenn aktiv: Diese Box droppt IMMER eine Ladder und sonst nix.")]
    public bool ladderOnlyMode = false;

    [Header("🪜 Ladder Drops")]
    [Tooltip("Liste der möglichen Ladder-Prefabs.")]
    public List<LadderDrop> possibleLadders = new List<LadderDrop>();

    [Tooltip("Spawn-Offset für die Ladder.")]
    public Vector2 ladderSpawnOffset = new Vector2(0f, 1f);

    [Tooltip("Wenn true, wird die Box beim Ladder-Drop zerstört.")]
    public bool forceDestroyBoxOnLadderDrop = true;

    [Header("🏁 PowerUp Drops")]
    [Range(0f, 1f)]
    public float globalPowerUpChance = 0.4f;

    public List<PowerUpDrop> possiblePowerUps = new List<PowerUpDrop>();

    [SerializeField]
    private float powerUpCooldownSeconds = 30f;

    [Header("💰 Coin Drop")]
    public GameObject coinPrefab;

    public float coinSpawnYOffset = 1.2f;

    [Tooltip("Wie viel soll der gespawnte Coin wert sein?")]
    public int spawnedCoinValue = 1;

    private static Dictionary<int, float> fragmentCooldownTimers =
        new Dictionary<int, float>();

    private static float lastDropTime = -9999f;

    private static GameObject lastSpawnedDrop = null;

    private static bool isQuitting = false;

    private bool isBroken = false;

    [Header("🔊 destruction-sound")]
    public AudioSource destructionSound;

    [Header("🔊 hit-sound")]
    public AudioSource hitSound;

    private void Start()
    {
        currentHitPoints = hitPoints;

        if (Random.value <= behaviorChances.startInvisibleChance)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null && !sr.enabled)
        {
            sr.enabled = true;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                ApplyHit();
                break;
            }
        }
    }

    private void ApplyHit()
    {
        if (isBroken)
            return;

        if (hitSound != null && !hitSound.isPlaying)
        {
            hitSound.Play();
        }

        currentHitPoints--;

        if (currentHitPoints <= 0)
        {
            isBroken = true;
            BreakBox();
        }
    }

    private void BreakBox()
    {
        if (!isBroken)
        {
            isBroken = true;
        }

        TrySpawnFragments();

        string currentLevel =
            SceneManager.GetActiveScene().name;

        // Punkte global hinzufügen
        int globalPoints =
            PlayerPrefs.GetInt("GlobalPoints", 0)
            + awardedPoints;

        PlayerPrefs.SetInt(
            "GlobalPoints",
            globalPoints
        );

        PlayerPrefsKeyTracker.TrackKey(
            "GlobalPoints"
        );

        // Punkte für aktuelles Level
        string levelPointsKey =
            $"{currentLevel}_Points";

        int levelPoints =
            PlayerPrefs.GetInt(levelPointsKey, 0)
            + awardedPoints;

        PlayerPrefs.SetInt(
            levelPointsKey,
            levelPoints
        );

        PlayerPrefsKeyTracker.TrackKey(
            levelPointsKey
        );

        PlayerPrefs.Save();

        CoinStatsDisplay statsDisplay =
            FindFirstObjectByType<CoinStatsDisplay>();

        if (statsDisplay != null)
        {
            statsDisplay.UpdatePointStats();
        }

        if (destructionSound != null &&
            !destructionSound.isPlaying)
        {
            destructionSound.Play();
        }

        // ============================
        // LADDER ONLY
        // ============================

        if (ladderOnlyMode)
        {
            SpawnLadderGuaranteed();

            if (forceDestroyBoxOnLadderDrop)
            {
                Destroy(gameObject);
            }
            else
            {
                SpriteRenderer sr =
                    GetComponent<SpriteRenderer>();

                if (sr != null &&
                    brokenSprite != null)
                {
                    sr.sprite = brokenSprite;
                }

                Collider2D col =
                    GetComponent<Collider2D>();

                if (col != null)
                {
                    col.enabled = false;
                }
            }

            return;
        }

        // ============================
        // NORMALER DROP
        // ============================

        TryDropReward();

        if (Random.value <=
            behaviorChances.shouldBreakChance)
        {
            if (Random.value <=
                behaviorChances.showDestructionEffectChance)
            {
                PlayDestructionEffect();
            }

            if (destroyOnBreak)
            {
                Destroy(gameObject);
            }
            else
            {
                SpriteRenderer sr =
                    GetComponent<SpriteRenderer>();

                if (sr != null &&
                    brokenSprite != null)
                {
                    sr.sprite = brokenSprite;
                }
            }
        }
        else
        {
            SpriteRenderer sr =
                GetComponent<SpriteRenderer>();

            if (sr != null &&
                brokenSprite != null)
            {
                sr.sprite = brokenSprite;
            }
        }
    }

    private void TrySpawnFragments()
    {
        if (fragmentsPrefabs == null ||
            fragmentsPrefabs.Length == 0)
        {
            return;
        }

        if (randomSettings != null &&
            Random.value >
            randomSettings.fragmentSpawnChance)
        {
            return;
        }

        float currentTime = Time.time;

        List<int> availableIndices =
            new List<int>();

        for (int i = 0;
             i < fragmentsPrefabs.Length;
             i++)
        {
            if (!fragmentCooldownTimers.ContainsKey(i) ||
                currentTime -
                fragmentCooldownTimers[i]
                >= fragmentCooldown)
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0)
            return;

        int randomIndex =
            Random.Range(
                0,
                availableIndices.Count
            );

        fragmentCooldownTimers[
            availableIndices[randomIndex]
        ] = currentTime;

        int selectedPrefabIndex =
            availableIndices[randomIndex];

        GameObject prefab =
            fragmentsPrefabs[selectedPrefabIndex];

        if (prefab == null)
            return;

        Vector3 spawnPos =
            transform.position +
            new Vector3(
                0f,
                fragmentSpawnYOffset,
                0f
            );

        GameObject fragments =
            Instantiate(
                prefab,
                spawnPos,
                transform.rotation
            );

        Transform[] allChildren =
            fragments.GetComponentsInChildren
            <Transform>(true);

        List<GameObject> childObjects =
            new List<GameObject>();

        for (int c = 1;
             c < allChildren.Length;
             c++)
        {
            childObjects.Add(
                allChildren[c].gameObject
            );
        }

        foreach (GameObject child
                 in childObjects)
        {
            child.SetActive(false);
        }

        if (childObjects.Count > 0)
        {
            int childIndex =
                Random.Range(
                    0,
                    childObjects.Count
                );

            childObjects[
                childIndex
            ].SetActive(true);
        }

        Rigidbody2D[] rigidbodies =
            fragments.GetComponentsInChildren
            <Rigidbody2D>();

        foreach (Rigidbody2D fragmentRB
                 in rigidbodies)
        {
            Vector2 dir =
                (
                    fragmentRB.transform.position
                    - transform.position
                ).normalized;

            fragmentRB.AddForce(
                dir * explosionForce,
                ForceMode2D.Impulse
            );
        }
    }

    private void TryDropReward()
    {
        if (Time.time - lastDropTime <
            powerUpCooldownSeconds)
        {
            return;
        }

        bool dropPowerUp =
            Random.value <=
            globalPowerUpChance
            &&
            possiblePowerUps != null
            &&
            possiblePowerUps.Count > 0;

        bool dropCoin =
            randomSettings != null
            &&
            Random.value <=
            randomSettings.coinAwardChance
            &&
            coinPrefab != null;

        // Es droppt manchmal gar nichts
        if (!dropPowerUp && !dropCoin)
        {
            Debug.Log(
                "📦 Box zerstört: Kein Drop."
            );

            return;
        }

        // Wenn beides möglich wäre,
        // wird nur EINS davon gewählt.
        bool givePowerUp =
            dropPowerUp &&
            (
                !dropCoin ||
                Random.value > 0.5f
            );

        if (givePowerUp)
        {
            SpawnPowerUp();
        }
        else
        {
            SpawnCoin();
        }
    }

    private void SpawnPowerUp()
    {
        List<GameObject> validDrops =
            new List<GameObject>();

        foreach (PowerUpDrop drop
                 in possiblePowerUps)
        {
            if (drop == null)
                continue;

            if (drop.powerUpPrefab == null)
                continue;

            if (drop.powerUpPrefab ==
                lastSpawnedDrop)
            {
                continue;
            }

            if (Random.value <=
                drop.individualChance)
            {
                validDrops.Add(
                    drop.powerUpPrefab
                );
            }
        }

        if (validDrops.Count == 0)
        {
            Debug.Log(
                "🎁 Kein gültiges PowerUp gewählt."
            );

            return;
        }

        GameObject selected =
            validDrops[
                Random.Range(
                    0,
                    validDrops.Count
                )
            ];

        lastDropTime = Time.time;
        lastSpawnedDrop = selected;

        Vector3 spawnPos =
            transform.position +
            new Vector3(
                0f,
                1.2f,
                0f
            );

        Instantiate(
            selected,
            spawnPos,
            Quaternion.identity
        );

        string itemName =
            selected.name;

        string powerUpKey =
            $"PowerUp_{itemName}";

        int currentAmount =
            PlayerPrefs.GetInt(
                powerUpKey,
                0
            ) + 1;

        PlayerPrefs.SetInt(
            powerUpKey,
            currentAmount
        );

        PlayerPrefsKeyTracker.TrackKey(
            powerUpKey
        );

        PlayerPrefs.Save();

        Debug.Log(
            $"🎁 PowerUp gespawnt: {itemName}"
        );
    }

    private void SpawnCoin()
    {
        if (coinPrefab == null)
            return;

        lastDropTime = Time.time;

        Vector3 spawnPos =
            transform.position +
            new Vector3(
                0f,
                coinSpawnYOffset,
                0f
            );

        GameObject coinObj =
            Instantiate(
                coinPrefab,
                spawnPos,
                Quaternion.identity
            );

        Coin pickup =
            coinObj.GetComponent<Coin>();

        if (pickup != null)
        {
            pickup.coinValue =
                Mathf.Max(
                    1,
                    spawnedCoinValue
                );
        }
        else
        {
            Debug.LogWarning(
                "⚠️ Das Coin-Prefab hat kein Coin-Script!"
            );
        }

        // WICHTIG:
        // Hier werden KEINE Coins
        // in PlayerPrefs hinzugefügt.
        //
        // Das passiert erst,
        // wenn der Spieler den Coin
        // tatsächlich einsammelt.

        Debug.Log(
            $"💰 Coin gespawnt. Wert: " +
            $"{Mathf.Max(1, spawnedCoinValue)}"
        );
    }

    private void SpawnLadderGuaranteed()
    {
        if (possibleLadders == null ||
            possibleLadders.Count == 0)
        {
            Debug.LogWarning(
                "🪜 Ladder-Only ist aktiv, " +
                "aber possibleLadders ist leer!"
            );

            return;
        }

        List<GameObject> valid =
            new List<GameObject>();

        foreach (LadderDrop ladderDrop
                 in possibleLadders)
        {
            if (ladderDrop == null)
                continue;

            if (ladderDrop.ladderPrefab == null)
                continue;

            if (Random.value <=
                Mathf.Clamp01(
                    ladderDrop.individualChance
                ))
            {
                valid.Add(
                    ladderDrop.ladderPrefab
                );
            }
        }

        GameObject chosen = null;

        if (valid.Count > 0)
        {
            chosen =
                valid[
                    Random.Range(
                        0,
                        valid.Count
                    )
                ];
        }
        else
        {
            // Fallback:
            // Ladder muss garantiert kommen
            foreach (LadderDrop ladderDrop
                     in possibleLadders)
            {
                if (ladderDrop != null &&
                    ladderDrop.ladderPrefab != null)
                {
                    chosen =
                        ladderDrop.ladderPrefab;

                    break;
                }
            }
        }

        if (chosen == null)
        {
            Debug.LogWarning(
                "🪜 Keine gültige Ladder gefunden."
            );

            return;
        }

        Vector3 spawnPos =
            transform.position +
            new Vector3(
                ladderSpawnOffset.x,
                ladderSpawnOffset.y,
                0f
            );

        Instantiate(
            chosen,
            spawnPos,
            Quaternion.identity
        );

        Debug.Log(
            $"🪜 Ladder gespawnt: " +
            $"{chosen.name}"
        );
    }

    private void PlayDestructionEffect()
    {
        if (isQuitting ||
            destructionEffectPrefab == null)
        {
            return;
        }

        GameObject fx =
            Instantiate(
                destructionEffectPrefab,
                transform.position,
                Quaternion.identity
            );

        ParticleSystem ps =
            fx.GetComponent<ParticleSystem>();

        float lifetime = 2f;

        if (ps != null)
        {
            lifetime =
                ps.main.duration;
        }

        Destroy(
            fx,
            lifetime
        );
    }
}

[System.Serializable]
public class LadderDrop
{
    public GameObject ladderPrefab;

    [Range(0f, 1f)]
    public float individualChance = 1f;
}