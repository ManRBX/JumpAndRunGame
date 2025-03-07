using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BreakableBox2D : MonoBehaviour
{
    // Anzahl der Treffer, bevor die Box als zerbrochen gilt.
    public int hitPoints = 1;
    private int currentHitPoints;

    // Array von Prefabs, die die zerbrochenen Fragmente enthalten (optional).
    public GameObject[] fragmentsPrefabs;

    // Die Kraft, die auf die Fragmente angewendet wird (falls verwendet).
    public float explosionForce = 300f;

    // Optional: Der Radius, in dem die Kraft angewendet wird (für weiteres Feintuning).
    public float explosionRadius = 2f;

    // Prefab für den Zerstörungseffekt (z. B. Partikeleffekt).
    public GameObject destructionEffectPrefab;

    // Das Sprite, das angezeigt wird, wenn die Box zerbrochen ist.
    public Sprite brokenSprite;

    // Bestimmt, ob die Box beim Zerbrechen zerstört wird.
    public bool destroyOnBreak = false;

    // Punkte, die immer vergeben werden.
    public int awardedPoints = 10;

    // Steuert Münzen- und Fragment-Spawning.
    public RandomSettings randomSettings;

    // Y-Offset für den Spawn der Fragmente (z. B. damit sie etwas über der Box erscheinen).
    [SerializeField] private float fragmentSpawnYOffset = 1f;

    // Cooldown in Sekunden für jedes Fragment (wie lange, bis es erneut spawnen darf).
    [SerializeField] private float fragmentCooldown = 20f;

    // Statisches Dictionary, das den letzten Spawn-Zeitpunkt je Fragmentindex speichert.
    private static Dictionary<int, float> fragmentCooldownTimers = new Dictionary<int, float>();

    // Flag, ob die Anwendung beendet wird (um Effekte beim Beenden zu verhindern).
    private static bool isQuitting = false;

    // Flag, ob die Box bereits zerbrochen ist.
    private bool isBroken = false;

    // Neuer Parameter: Soll die Box am Anfang unsichtbar sein?
    public bool startInvisible = false;

    void Start()
    {
        // Initialisiere die aktuellen Trefferpunkte.
        currentHitPoints = hitPoints;

        // Falls die Box unsichtbar starten soll, deaktiviere den SpriteRenderer.
        if (startInvisible)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }

    // Wird aufgerufen, wenn die Anwendung beendet wird.
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // Wird aufgerufen, wenn es zu einer 2D-Kollision kommt.
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Überprüfe, ob das kollidierende Objekt den Tag "Player" hat.
        if (collision.gameObject.CompareTag("Player"))
        {
            // Falls die Box unsichtbar ist, mache sie sichtbar.
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && !sr.enabled)
            {
                sr.enabled = true;
            }

            // Gehe alle Kontaktpunkte der Kollision durch.
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Prüfe, ob der Kontakt von oben kommt.
                if (contact.normal.y > 0.5f)
                {
                    ApplyHit();
                    break;
                }
            }
        }
    }

    // Verringert die Trefferpunkte und löst das Zerbrechen aus, wenn keine Trefferpunkte mehr vorhanden sind.
    void ApplyHit()
    {
        // Wenn bereits kaputt, nichts mehr machen.
        if (isBroken)
            return;

        currentHitPoints--;

        // Wenn die Box jetzt kaputt ist, sofort als kaputt markieren und BreakBox aufrufen.
        if (currentHitPoints <= 0)
        {
            isBroken = true;
            BreakBox();
        }
    }

    // Methode, die das Zerbrechen der Box behandelt.
    void BreakBox()
    {
        // Doppelte Absicherung.
        if (!isBroken)
            isBroken = true;

        // Spawne Fragmente nur, wenn die Zufallschance es zulässt.
        if (fragmentsPrefabs != null && fragmentsPrefabs.Length > 0)
        {
            if (Random.value <= randomSettings.fragmentSpawnChance)
            {
                // Erstelle eine Liste der Indizes, deren Cooldown abgelaufen ist.
                List<int> availableIndices = new List<int>();
                float currentTime = Time.time;

                for (int i = 0; i < fragmentsPrefabs.Length; i++)
                {
                    // Wenn es noch keinen Eintrag gibt oder der letzte Spawn schon länger als fragmentCooldown her ist:
                    if (!fragmentCooldownTimers.ContainsKey(i) ||
                        (currentTime - fragmentCooldownTimers[i]) >= fragmentCooldown)
                    {
                        availableIndices.Add(i);
                    }
                }

                // Wenn mindestens ein Fragment verfügbar ist, wähle zufällig eines aus.
                if (availableIndices.Count > 0)
                {
                    int randomAvailableIndex = availableIndices[Random.Range(0, availableIndices.Count)];
                    fragmentCooldownTimers[randomAvailableIndex] = currentTime; // Setze den Cooldown für dieses Fragment

                    GameObject selectedPrefab = fragmentsPrefabs[randomAvailableIndex];

                    // Berechne die Spawn-Position mit Y-Offset (über der Box)
                    Vector3 spawnPosition = transform.position + new Vector3(0, fragmentSpawnYOffset, 0);
                    GameObject fragments = Instantiate(selectedPrefab, spawnPosition, transform.rotation);

                    // Deaktiviere alle Kinder und aktiviere nur **eines** zufällig
                    Transform[] allChildren = fragments.GetComponentsInChildren<Transform>(true);
                    List<GameObject> childObjects = new List<GameObject>();

                    // Index 0 ist normalerweise das Parent selbst (das Prefab), deshalb ab 1 starten
                    for (int c = 1; c < allChildren.Length; c++)
                    {
                        childObjects.Add(allChildren[c].gameObject);
                    }

                    // Erstmal alle Kinder deaktivieren
                    foreach (GameObject child in childObjects)
                    {
                        child.SetActive(false);
                    }

                    // Nur ein Kind zufällig aktivieren
                    if (childObjects.Count > 0)
                    {
                        int randomChildIndex = Random.Range(0, childObjects.Count);
                        childObjects[randomChildIndex].SetActive(true);
                    }

                    // Wende eine Explosionskraft auf alle Rigidbody2D-Komponenten in den aktiven Teilen an.
                    foreach (Rigidbody2D rb in fragments.GetComponentsInChildren<Rigidbody2D>())
                    {
                        Vector2 direction = (rb.transform.position - transform.position).normalized;
                        rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        // Ändere das Sprite, um anzuzeigen, dass die Box zerbrochen ist.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && brokenSprite != null)
        {
            sr.sprite = brokenSprite;
        }

        // Vergib Punkte.
        int globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);
        globalPoints += awardedPoints;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);

        string currentLevel = SceneManager.GetActiveScene().name;
        int levelPoints = PlayerPrefs.GetInt($"{currentLevel}_Points", 0);
        levelPoints += awardedPoints;
        PlayerPrefs.SetInt($"{currentLevel}_Points", levelPoints);

        // Vergib Münzen, wenn die Zufallschance es zulässt.
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

        // Aktualisiere die UI, falls ein CoinStatsDisplay vorhanden ist.
        CoinStatsDisplay statsDisplay = FindFirstObjectByType<CoinStatsDisplay>();
        if (statsDisplay != null)
        {
            statsDisplay.UpdatePointStats();
        }

        // Wenn destroyOnBreak true ist, spiele den Zerstörungseffekt ab und zerstöre die Box.
        if (destroyOnBreak)
        {
            PlayDestructionEffect();
            Destroy(gameObject);
        }
    }

    // Methode, um den Zerstörungseffekt abzuspielen.
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
