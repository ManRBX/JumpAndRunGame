using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BreakableBox2D : MonoBehaviour
{
    // Number of hits required before the box is considered broken.
    public int hitPoints = 1;
    private int currentHitPoints;

    // Array of prefabs containing the broken fragments (optional).
    public GameObject[] fragmentsPrefabs;

    // The force applied to the fragments (if used).
    public float explosionForce = 300f;

    // Optional: The radius within which the force is applied (for fine tuning).
    public float explosionRadius = 2f;

    // Prefab for the destruction effect (e.g., particle effect).
    public GameObject destructionEffectPrefab;

    // The sprite that will be shown when the box is broken.
    public Sprite brokenSprite;

    // Determines whether the box is destroyed upon breaking.
    public bool destroyOnBreak = false;

    // Points that are always awarded.
    public int awardedPoints = 10;

    // Controls coin and fragment spawning.
    public RandomSettings randomSettings;

    // Y-offset for fragment spawn position (so they appear slightly above the box).
    [SerializeField] private float fragmentSpawnYOffset = 1f;

    // Cooldown in seconds for each fragment (how long until it can spawn again).
    [SerializeField] private float fragmentCooldown = 20f;

    // Static dictionary storing the last spawn time for each fragment index.
    private static Dictionary<int, float> fragmentCooldownTimers = new Dictionary<int, float>();

    // Flag indicating if the application is quitting (to prevent playing effects on quit).
    private static bool isQuitting = false;

    // Flag indicating whether the box is already broken.
    private bool isBroken = false;

    // New parameter: Should the box start off invisible?
    public bool startInvisible = false;

    void Start()
    {
        // Initialize current hit points.
        currentHitPoints = hitPoints;

        // If the box should start invisible, disable the SpriteRenderer.
        if (startInvisible)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }

    // Called when the application is quitting.
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // Called when a 2D collision occurs.
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object has the tag "Player".
        if (collision.gameObject.CompareTag("Player"))
        {
            // If the box is invisible, make it visible.
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && !sr.enabled)
            {
                sr.enabled = true;
            }

            // Iterate through all contact points.
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // If the contact comes from above.
                if (contact.normal.y > 0.5f)
                {
                    ApplyHit();
                    break;
                }
            }
        }
    }

    // Reduces the hit points and triggers the break if no hit points remain.
    void ApplyHit()
    {
        // If already broken, do nothing.
        if (isBroken)
            return;

        currentHitPoints--;

        // If hit points drop to zero or below, mark the box as broken and break it.
        if (currentHitPoints <= 0)
        {
            isBroken = true;
            BreakBox();
        }
    }

    // Handles the breaking of the box.
    void BreakBox()
    {
        // Ensure the box is marked as broken.
        if (!isBroken)
            isBroken = true;

        // Spawn fragments if prefabs are provided.
        if (fragmentsPrefabs != null && fragmentsPrefabs.Length > 0)
        {
            if (Random.value <= randomSettings.fragmentSpawnChance)
            {
                // Create a list of indices whose cooldown has expired.
                List<int> availableIndices = new List<int>();
                float currentTime = Time.time;

                for (int i = 0; i < fragmentsPrefabs.Length; i++)
                {
                    // If there is no previous spawn record or the cooldown has expired:
                    if (!fragmentCooldownTimers.ContainsKey(i) ||
                        (currentTime - fragmentCooldownTimers[i]) >= fragmentCooldown)
                    {
                        availableIndices.Add(i);
                    }
                }

                // If at least one fragment is available, choose one randomly.
                if (availableIndices.Count > 0)
                {
                    int randomAvailableIndex = availableIndices[Random.Range(0, availableIndices.Count)];
                    fragmentCooldownTimers[randomAvailableIndex] = currentTime; // Set the cooldown for this fragment

                    GameObject selectedPrefab = fragmentsPrefabs[randomAvailableIndex];

                    // Calculate the spawn position with a Y-offset (above the box)
                    Vector3 spawnPos = transform.position + new Vector3(0, fragmentSpawnYOffset, 0);
                    GameObject fragments = Instantiate(selectedPrefab, spawnPos, transform.rotation);

                    // Disable all children and only activate one randomly.
                    Transform[] allChildren = fragments.GetComponentsInChildren<Transform>(true);
                    List<GameObject> childObjects = new List<GameObject>();

                    // Index 0 is usually the parent itself, so start from 1.
                    for (int c = 1; c < allChildren.Length; c++)
                    {
                        childObjects.Add(allChildren[c].gameObject);
                    }

                    // Disable all child objects.
                    foreach (GameObject child in childObjects)
                    {
                        child.SetActive(false);
                    }

                    // Activate one random child.
                    if (childObjects.Count > 0)
                    {
                        int randomChildIndex = Random.Range(0, childObjects.Count);
                        childObjects[randomChildIndex].SetActive(true);
                    }

                    // Apply explosion force to all Rigidbody2D components in the active fragments.
                    foreach (Rigidbody2D rb in fragments.GetComponentsInChildren<Rigidbody2D>())
                    {
                        Vector2 direction = (rb.transform.position - transform.position).normalized;
                        rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        // Change the sprite to indicate the box is broken.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && brokenSprite != null)
        {
            spriteRenderer.sprite = brokenSprite;
        }

        // Award points.
        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        globalPoints += awardedPoints;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);

        string currentLevel = SceneManager.GetActiveScene().name;
        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        levelPoints += awardedPoints;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);

        // Award coins if the random chance allows.
        if (Random.value <= randomSettings.coinAwardChance)
        {
            int globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
            globalCoins += randomSettings.awardedCoins;
            PlayerPrefs.SetInt("GlobalCoins", globalCoins);

            int levelCoins = PlayerPrefs.GetInt($"{currentLevel}_Coins", 0);
            levelCoins += randomSettings.awardedCoins;
            PlayerPrefs.SetInt($"{currentLevel}_Coins", levelCoins);
        }
        PlayerPrefs.Save();

        // Update the UI if a CoinStatsDisplay is present.
        CoinStatsDisplay statsDisplay = FindFirstObjectByType<CoinStatsDisplay>();
        if (statsDisplay != null)
        {
            statsDisplay.UpdatePointStats();
        }

        // If destroyOnBreak is true, play the destruction effect and destroy the box.
        if (destroyOnBreak)
        {
            PlayDestructionEffect();
            Destroy(gameObject);
        }
    }

    // Plays the destruction effect.
    void PlayDestructionEffect()
    {
        if (isQuitting)
            return;

        if (destructionEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effectInstance, ps.main.duration);
            }
            else
            {
                Destroy(effectInstance, 2f);
            }
        }
    }
}
