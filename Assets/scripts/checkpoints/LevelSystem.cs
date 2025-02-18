using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    public Button[] levelButtons;  // Array for level buttons in the menu

    void Start()
    {
        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            string levelKey = "Level" + (i + 1).ToString("00") + "_Unlocked";

            // Level01 is always unlocked
            if (i == 0)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = PlayerPrefs.GetInt(levelKey, 0) == 1;
            }
        }
    }

    public void LoadLevelByName(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("Scene '" + levelName + "' does not exist or is not in the build!");
        }
    }
}
