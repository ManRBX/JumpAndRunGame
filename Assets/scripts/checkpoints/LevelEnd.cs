using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public string nextLevelName;  // Name of the next level (e.g., "Level02")
    public string currentLevelName;  // Current level (e.g., "Level01")
    public string returnScene = "Menu";  // Scene to return to

    private void Start()
    {
        // Level01 is always unlocked
        if (currentLevelName == "Level01")
        {
            PlayerPrefs.SetInt(currentLevelName + "_Unlocked", 1);
        }

        // Mark the level as visited
        PlayerPrefs.SetInt(currentLevelName + "_Visited", 1);
        PlayerPrefs.Save();
        Debug.Log(currentLevelName + " entered.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Save level as completed
            PlayerPrefs.SetInt(currentLevelName + "_Completed", 1);
            Debug.Log(currentLevelName + " completed!");

            // Unlock the next level (unless it's the last level)
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                PlayerPrefs.SetInt(nextLevelName + "_Unlocked", 1);
                Debug.Log(nextLevelName + " unlocked!");
            }

            PlayerPrefs.Save();

            // Return to the menu
            SceneManager.LoadScene(returnScene);
        }
    }
}
