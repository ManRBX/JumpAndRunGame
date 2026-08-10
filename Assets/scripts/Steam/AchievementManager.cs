using UnityEngine;
using Steamworks;
using System;

[Serializable]
public class AchievementEntry
{
    [Header("Steam Achievement")]
    public string achievementID;

    [Header("PlayerPrefs Key")]
    public string playerPrefsKey;

    [Header("Vergleichswert")]
    public int requiredValue = 1;

    [Header("Vergleich")]
    public ComparisonType comparison = ComparisonType.GreaterOrEqual;
}

public enum ComparisonType
{
    Equal,
    GreaterOrEqual,
    Greater,
    LessOrEqual,
    Less
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [Header("Alle Achievements")]
    public AchievementEntry[] achievements;

    [Header("Prüfintervall")]
    public float checkInterval = 1f;

    private float timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckAchievements();
        }
    }

    private void CheckAchievements()
    {
        if (!SteamManager.Initialized)
            return;

        foreach (AchievementEntry achievement in achievements)
        {
            if (string.IsNullOrEmpty(achievement.achievementID))
                continue;

            bool alreadyUnlocked;
            SteamUserStats.GetAchievement(achievement.achievementID, out alreadyUnlocked);

            if (alreadyUnlocked)
                continue;

            int value = PlayerPrefs.GetInt(achievement.playerPrefsKey, 0);

            bool unlock = false;

            switch (achievement.comparison)
            {
                case ComparisonType.Equal:
                    unlock = value == achievement.requiredValue;
                    break;

                case ComparisonType.GreaterOrEqual:
                    unlock = value >= achievement.requiredValue;
                    break;

                case ComparisonType.Greater:
                    unlock = value > achievement.requiredValue;
                    break;

                case ComparisonType.LessOrEqual:
                    unlock = value <= achievement.requiredValue;
                    break;

                case ComparisonType.Less:
                    unlock = value < achievement.requiredValue;
                    break;
            }

            if (unlock)
            {
                SteamUserStats.SetAchievement(achievement.achievementID);
                SteamUserStats.StoreStats();

                Debug.Log("🏆 Achievement freigeschaltet: " + achievement.achievementID);
            }
        }
    }

    public void UnlockAchievement(string achievementID)
    {
        if (!SteamManager.Initialized)
            return;

        bool alreadyUnlocked;
        SteamUserStats.GetAchievement(achievementID, out alreadyUnlocked);

        if (alreadyUnlocked)
            return;

        SteamUserStats.SetAchievement(achievementID);
        SteamUserStats.StoreStats();

        Debug.Log("🏆 Achievement freigeschaltet: " + achievementID);
    }
}