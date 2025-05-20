using UnityEngine;
using Steamworks;
using System;

public class SteamAchievementManager : MonoBehaviour
{
    public static void Unlock(string achievementID)
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam nicht initialisiert.");
            return;
        }

        bool achieved;
        SteamUserStats.GetAchievement(achievementID, out achieved);

        if (!achieved)
        {
            SteamUserStats.SetAchievement(achievementID);
            SteamUserStats.StoreStats();
            Debug.Log($"✅ Achievement '{achievementID}' freigeschaltet!");
        }
        else
        {
            Debug.Log($"ℹ️ Achievement '{achievementID}' war schon freigeschaltet.");
        }
    }

    internal static void Unlock(object levelID)
    {
        throw new NotImplementedException();
    }
}
