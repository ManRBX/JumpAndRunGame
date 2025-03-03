using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BreakableBox2D : MonoBehaviour
{
    // Number of hits before the box is considered broken.
    // One hit is enough because the player stomps on the box from above.
    public int hitPoints = 1;

    // Array of prefabs that contain the broken fragments (optional).
    // You can add multiple prefabs here, one of which will be randomly selected.
    public GameObject[] fragmentsPrefabs;

    // The force applied to the fragments (if used).
    public float explosionForce = 300f;

    // Optional: The radius in which the force is applied (for further adjustments).
    public float explosionRadius = 2f;

    // Prefab for the destruction effect (e.g., particle effect).
    public GameObject destructionEffectPrefab;

    // The sprite to display when the box is broken.
    public Sprite brokenSprite;

    // If true, the box will be destroyed and the destruction effect will be played when breaking.
    // If false, only the sprite is changed (no effect, no destruction).
    public bool destroyOnBreak = false;

    // Points are always awarded.
    public int awardedPoints = 10;

    // Use the RandomSettings to control coins and fragment spawning.
    public RandomSettings randomSettings;

    // Flag to indicate if the application is quitting.
    private static bool isQuitting = false;

    // Flag to indicate if the box is already broken.
    private bool isBroken = false;

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
            // Loop through all contact points of the collision.
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Check if the contact comes from above.
                if (contact.normal.y > 0.5f)
                {
                    BreakBox();
                    break;
                }
            }
        }
    }

    // Method that handles breaking the box.
    void BreakBox()
    {
        if (isBroken)
            return;

        isBroken = true;

        // Spawn fragment only if the random chance passes.
        int selectedIndex = -1;
        if (fragmentsPrefabs != null && fragmentsPrefabs.Length > 0)
        {
            if (Random.value <= randomSettings.fragmentSpawnChance)
            {
                selectedIndex = Random.Range(0, fragmentsPrefabs.Length);
                GameObject selectedPrefab = fragmentsPrefabs[selectedIndex];
                GameObject fragments = Instantiate(selectedPrefab, transform.position, transform.rotation);

                // Apply an explosion force to all Rigidbody2D components in the fragments.
                foreach (Rigidbody2D rb in fragments.GetComponentsInChildren<Rigidbody2D>())
                {
                    Vector2 direction = (rb.transform.position - transform.position).normalized;
                    rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                }
            }
        }

        // Change the sprite to indicate that the box is broken.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && brokenSprite != null)
        {
            sr.sprite = brokenSprite;
        }

        // Always award points.
        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        globalPoints += awardedPoints;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);

        string currentLevel = SceneManager.GetActiveScene().name;
        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        levelPoints += awardedPoints;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);

        // Award coins only if the random chance passes.
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

        // Update UI if a CoinStatsDisplay exists.
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

    // Method to play the destruction effect.
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
