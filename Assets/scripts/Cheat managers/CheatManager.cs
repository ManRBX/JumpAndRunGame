using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class CheatManager : MonoBehaviour
{
    [Header("Cheat Settings")]
    public KeyCode toggleInvincibilityKey = KeyCode.I;
    public KeyCode addLivesKey = KeyCode.L;
    public KeyCode nextLevelKey = KeyCode.N;
    public KeyCode unlockAllAchievementsKey = KeyCode.U;
    public KeyCode speedBoostKey = KeyCode.B;

    private bool isInvincible = false;

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    void Start()
    {
        UpdateReferences();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleInvincibilityKey))
        {
            isInvincible = !isInvincible;
            Debug.Log("🛡️ Invincibility: " + (isInvincible ? "Aktiviert" : "Deaktiviert"));
            if (playerHealth != null)
                StartCoroutine(playerHealth.ApplyInvincibility(isInvincible ? 99999f : 0.1f));
        }

        if (Input.GetKeyDown(addLivesKey))
        {
            if (playerHealth != null)
            {
                playerHealth.AddLife(1);
                Debug.Log("❤️ Leben +1");
            }
        }

        if (Input.GetKeyDown(nextLevelKey))
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = index + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
                Debug.Log("➡️ Springe zu Level: " + nextIndex);
            }
        }

        if (Input.GetKeyDown(unlockAllAchievementsKey) && SteamManager.Initialized)
        {
            uint count = SteamUserStats.GetNumAchievements();
            for (uint i = 0; i < count; i++)
            {
                string achId = SteamUserStats.GetAchievementName(i);
                SteamUserStats.SetAchievement(achId);
            }
            SteamUserStats.StoreStats();
            Debug.Log("🏆 Alle Achievements freigeschaltet!");
        }

        if (Input.GetKeyDown(speedBoostKey))
        {
            if (playerMovement != null)
            {
                StartCoroutine(playerMovement.ApplySpeedBoost(2f, 5f));
                Debug.Log("⚡ Speed Boost für 5 Sekunden aktiviert!");
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateReferences();
    }

    void UpdateReferences()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }
}
