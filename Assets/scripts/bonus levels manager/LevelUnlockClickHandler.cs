using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUnlockClickHandler : MonoBehaviour
{
    [Header("⚙️ Einstellungen")]
    [Tooltip("Wenn aktiv, ist Level01 immer freigeschaltet.")]
    public bool level01AlwaysUnlocked = true;

    public void TryLoadLevel(string levelName)
    {
        if (string.IsNullOrWhiteSpace(levelName))
        {
            Debug.LogWarning("⚠️ Kein Levelname angegeben.");
            return;
        }

        string unlockKey = levelName + "_Unlocked";

        bool isUnlocked = PlayerPrefs.GetInt(unlockKey, 0) == 1;

        // Level01 optional immer erlauben
        if (level01AlwaysUnlocked && levelName == "Level01")
        {
            isUnlocked = true;
        }

        if (!isUnlocked)
        {
            Debug.LogWarning(
                $"🔒 Level '{levelName}' ist nicht freigeschaltet. " +
                $"Gesuchter Key: {unlockKey}"
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(levelName))
        {
            Debug.LogError(
                $"❌ Szene '{levelName}' wurde nicht gefunden oder ist nicht in den Build Settings."
            );

            return;
        }

        Debug.Log(
            $"✅ Lade Level: {levelName} | Unlock-Key: {unlockKey}"
        );

        SceneManager.LoadScene(levelName);
    }
}