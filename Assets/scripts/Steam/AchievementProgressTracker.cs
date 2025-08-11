using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
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
        TryTrackKey("CoinsCollected");
        PlayerPrefs.Save();
        CheckProgressAchievements();
    }

    public void AddKill()
    {
        enemiesKilled++;
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        TryTrackKey("EnemiesKilled");
        PlayerPrefs.Save();
        CheckProgressAchievements();
    }

    private void CheckProgressAchievements()
    {
        foreach (var coinAch in coinAchievements)
        {
            if (!coinAch.unlocked && coinsCollected >= coinAch.requiredAmount)
            {
                UnlockProgressAch(coinAch, "achv_coin_");
            }
        }

        foreach (var killAch in killAchievements)
        {
            if (!killAch.unlocked && enemiesKilled >= killAch.requiredAmount)
            {
                UnlockProgressAch(killAch, "achv_kill_");
            }
        }
    }

    private void UnlockProgressAch(ProgressAchievement ach, string prefix)
    {
        SteamAchievementManager.Unlock(ach.achievementID);
        ach.unlocked = true;
        PlayerPrefs.SetInt(prefix + ach.achievementID, 1);
        TryTrackKey(prefix + ach.achievementID);
        PlayerPrefs.Save();
        Debug.Log($"✅ Progress-Achievement freigeschaltet: {ach.achievementID}");
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

    /// <summary>
    /// Baut den PlayerPrefs-Key sauber als id + keyPrefix und schickt achievementID an Steam.
    /// </summary>
    private void UnlockFlag(string id, List<AchievementFlag> list, string keyPrefix, string achievementID)
    {
        string key = id + keyPrefix; // ✅ FIX: Suffix hinten anhängen

        foreach (var entry in list)
        {
            if (entry.ID.Equals(id, System.StringComparison.OrdinalIgnoreCase) && !PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                TryTrackKey(key);
                PlayerPrefs.Save();
                SteamAchievementManager.Unlock(achievementID);
                entry.unlocked = true;
                Debug.Log($"✅ Achievement freigeschaltet: {achievementID}");
                return;
            }
        }

        // Falls ID nicht in Liste, trotzdem setzen
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 1);
            TryTrackKey(key);
            PlayerPrefs.Save();
            SteamAchievementManager.Unlock(achievementID);
            Debug.LogWarning($"⚠️ ID '{id}' nicht in Liste, Achievement trotzdem freigeschaltet: {achievementID}");
        }
    }

    private void CheckFlags(List<AchievementFlag> list, string keyPrefix)
    {
        foreach (var entry in list)
        {
            string key = entry.ID + keyPrefix; // ✅ auch hier Suffix hinten
            if (PlayerPrefs.HasKey(key))
            {
                entry.unlocked = true;
            }
        }
    }

    private void TryTrackKey(string key)
    {
        try
        {
            var t = System.Type.GetType("PlayerPrefsKeyTracker");
            if (t == null) return;
            var m = t.GetMethod("TrackKey", BindingFlags.Public | BindingFlags.Static);
            if (m == null) return;
            m.Invoke(null, new object[] { key });
        }
        catch { /* still safe */ }
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
