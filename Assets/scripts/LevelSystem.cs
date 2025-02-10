using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    public Button[] levelButtons;  // Array für Level-Buttons im Menü

    void Start()
    {
        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            string levelKey = "Level" + (i + 1).ToString("00") + "_Freigeschaltet";

            // Level01 bleibt immer freigeschaltet
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
            Debug.LogError("Szene '" + levelName + "' existiert nicht oder ist nicht im Build!");
        }
    }
}
