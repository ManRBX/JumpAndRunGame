using UnityEngine;
using System.Collections.Generic;
using Steamworks;

public class AchievementProgressTracker : MonoBehaviour
{
    public static AchievementProgressTracker Instance;

    [Header("🔁 Coin-Achievements")]
    public List<ProgressAchievement> coinAchievements;

    [Header("🔁 Kill-Achievements")]
    public List<ProgressAchievement> killAchievements;

    [Header("🎯 Level-Abschluss-Achievements")]
    public List<AchievementFlag> completedLevels;

    [Header("🔓 Freigeschaltete Levels")]
    public List<AchievementFlag> unlockedLevels;

    [Header("🕵️ Geheimräume entdeckt")]
    public List<AchievementFlag> secretRooms;

    [Header("🔍 Objekte gefunden")]
    public List<AchievementFlag> objectFound;

    [Header("🎮 Bonus-Level gefunden")]
    public List<AchievementFlag> bonusLevels;

    private int coinsCollected;
    private int enemiesKilled;

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

        CheckProgressAchievements();
        CheckFlags(completedLevels, "_Completed");
        CheckFlags(unlockedLevels, "_Unlocked");
        CheckFlags(secretRooms, "SecretFound_");
        CheckFlags(objectFound, "ObjectFound_");
        CheckFlags(bonusLevels, "BonusLevelFound_");
    }

    public void AddCoin()
    {
        coinsCollected++;
        PlayerPrefs.SetInt("CoinsCollected", coinsCollected);
        CheckProgressAchievements();
    }

    public void AddKill()
    {
        enemiesKilled++;
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        CheckProgressAchievements();
    }

    private void CheckProgressAchievements()
    {
        foreach (var coinAch in coinAchievements)
        {
            if (!coinAch.unlocked && coinsCollected >= coinAch.requiredAmount)
            {
                SteamAchievementManager.Unlock(coinAch.achievementID);
                coinAch.unlocked = true;
            }
        }

        foreach (var killAch in killAchievements)
        {
            if (!killAch.unlocked && enemiesKilled >= killAch.requiredAmount)
            {
                SteamAchievementManager.Unlock(killAch.achievementID);
                killAch.unlocked = true;
            }
        }
    }

    public void OnLevelCompleted(string levelID)
    {
        UnlockFlag(levelID, completedLevels, "_Completed", levelID);
    }

    public void OnLevelUnlocked(string levelID)
    {
        UnlockFlag(levelID, unlockedLevels, "_Unlocked", levelID);
    }

    public void OnSecretFound(string secretID)
    {
        UnlockFlag(secretID, secretRooms, "SecretFound_", secretID);
    }

    public void OnObjectFound(string objectID)
    {
        UnlockFlag(objectID, objectFound, "ObjectFound_", objectID);
    }

    public void OnBonusLevelFound(string bonusLevelID)
    {
        UnlockFlag(bonusLevelID, bonusLevels, "BonusLevelFound_", bonusLevelID);
    }

    private void UnlockFlag(string id, List<AchievementFlag> list, string keyPrefix, string achievementID)
    {
        string key = keyPrefix + id;
        foreach (var entry in list)
        {
            if (entry.ID.Equals(id, System.StringComparison.OrdinalIgnoreCase) && !PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                SteamAchievementManager.Unlock(achievementID);
                entry.unlocked = true;
                Debug.Log($"✅ Achievement freigeschaltet: {achievementID}");
                break;
            }
        }
    }

    private void CheckFlags(List<AchievementFlag> list, string keyPrefix)
    {
        foreach (var entry in list)
        {
            string key = keyPrefix + entry.ID;
            if (PlayerPrefs.HasKey(key))
            {
                entry.unlocked = true;
            }
        }
    }
}

[System.Serializable]
public class ProgressAchievement
{
    public string achievementID;
    public int requiredAmount;
    [HideInInspector] public bool unlocked;
}

[System.Serializable]
public class AchievementFlag
{
    public string ID;
    [HideInInspector] public bool unlocked;
}
