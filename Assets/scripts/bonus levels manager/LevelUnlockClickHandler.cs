using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUnlockClickHandler : MonoBehaviour
{
    public void TryLoadLevel(string levelName)
    {
        string unlockKey = levelName + "_Unlocked";
        bool isUnlocked = PlayerPrefs.GetInt(unlockKey, 0) == 1;

        if (!isUnlocked)
        {
            Debug.LogWarning("❌ Bonus-Level '" + levelName + "' ist nicht freigeschaltet.");
            return;
        }

        Debug.Log("✅ Lade Bonus-Level: " + levelName);
        SceneManager.LoadScene(levelName);
    }
}
