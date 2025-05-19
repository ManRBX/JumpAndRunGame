using UnityEngine;

public class AchievementProgressTracker : MonoBehaviour
{
    public static AchievementProgressTracker Instance;

    public int coinsCollected = 0;
    public int enemiesKilled = 0;

    private bool coinAchievementUnlocked = false;
    private bool killAchievementUnlocked = false;

    [Header("Ziele")]
    public int coinGoal = 100;
    public int killGoal = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        coinsCollected = PlayerPrefs.GetInt("CoinsCollected", 0);
        enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);

        CheckAchievements();
    }

    public void AddCoin()
    {
        coinsCollected++;
        PlayerPrefs.SetInt("CoinsCollected", coinsCollected);
        CheckAchievements();
    }

    public void AddKill()
    {
        enemiesKilled++;
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        CheckAchievements();
    }

    private void CheckAchievements()
    {
        if (!coinAchievementUnlocked && coinsCollected >= coinGoal)
        {
            SteamAchievementManager.Unlock("COLLECT_100_COINS");
            coinAchievementUnlocked = true;
        }

        if (!killAchievementUnlocked && enemiesKilled >= killGoal)
        {
            SteamAchievementManager.Unlock("KILL_10_ENEMIES");
            killAchievementUnlocked = true;
        }
    }
}
