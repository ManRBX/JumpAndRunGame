using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    [Header("Normale Level")]
    public Button[] levelButtons;
    public string[] levelNames;

    [Header("Bonus-Level")]
    public Button[] bonusButtons;
    public string[] bonusLevelNames;

    void Start()
    {
        UpdateLevelButtons();
        UpdateBonusLevelButtons();
    }

    void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length && i < levelNames.Length; i++)
        {
            string levelKey = levelNames[i] + "_Unlocked";

            if (i == 0)
            {
                levelButtons[i].interactable = true; // erstes Level ist immer frei
            }
            else
            {
                levelButtons[i].interactable = PlayerPrefs.GetInt(levelKey, 0) == 1;
            }
        }
    }

    void UpdateBonusLevelButtons()
    {
        for (int i = 0; i < bonusButtons.Length && i < bonusLevelNames.Length; i++)
        {
            string bonusKey = bonusLevelNames[i] + "_Unlocked";
            bonusButtons[i].interactable = PlayerPrefs.GetInt(bonusKey, 0) == 1;
        }
    }

    public void LoadLevelByName(string levelName)
    {
        string unlockKey = levelName + "_Unlocked";
        bool isUnlocked = PlayerPrefs.GetInt(unlockKey, 0) == 1;

        if (!isUnlocked)
        {
            Debug.LogWarning("❌ Zugriff auf '" + levelName + "' verweigert – nicht freigeschaltet! (Key: " + unlockKey + ")");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            Debug.Log("✅ Lade Level: " + levelName);
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("❌ Szene '" + levelName + "' ist nicht im Build enthalten!");
        }
    }
}
