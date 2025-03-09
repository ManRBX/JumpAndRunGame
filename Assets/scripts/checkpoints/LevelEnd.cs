using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelEnd : MonoBehaviour
{
    public string nextLevelName;  // Name of the next level (e.g., "Level02")
    public string currentLevelName;  // Current level (e.g., "Level01")
    public string returnScene = "Menu";  // Scene to return to after completing the level
    public int requiredSpecialCoins = 5; // Total number of special coins required to unlock the next level

    public TMP_Text coinProgressText; // UI element for displaying coin progress

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

        // Hide the coin progress UI at the start
        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(false);
        }
    }

    private void UpdateCoinProgressUI()
    {
        // Load the total number of collected special coins from GlobalSpecialCoins
        int collectedGlobalSpecialCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);

        // Update the UI with the format "Collected / Required" (e.g., "3/5")
        if (coinProgressText != null)
        {
            coinProgressText.text = $"{collectedGlobalSpecialCoins}/{requiredSpecialCoins}";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Load the total amount of GlobalSpecialCoins
            int collectedGlobalSpecialCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);

            // Check if the required number of special coins has been collected
            if (collectedGlobalSpecialCoins >= requiredSpecialCoins)
            {
                // Mark the level as completed
                PlayerPrefs.SetInt(currentLevelName + "_Completed", 1);
                Debug.Log(currentLevelName + " completed!");

                // Unlock the next level (if it exists)
                if (!string.IsNullOrEmpty(nextLevelName))
                {
                    PlayerPrefs.SetInt(nextLevelName + "_Unlocked", 1);
                    Debug.Log(nextLevelName + " unlocked!");
                }

                PlayerPrefs.Save();

                // Load the menu scene
                SceneManager.LoadScene(returnScene);
            }
            else
            {
                Debug.Log($"❌ Not enough special coins! You need {requiredSpecialCoins}, but only have {collectedGlobalSpecialCoins}.");
            }

            // Show the coin progress UI and hide it after 5 seconds
            ShowCoinProgress();
        }
    }

    void ShowCoinProgress()
    {
        // Update the coin progress UI
        UpdateCoinProgressUI();

        // Show the coin progress UI
        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(true);
        }

        // Hide the UI after 5 seconds
        StartCoroutine(HideCoinProgress());
    }

    IEnumerator HideCoinProgress()
    {
        yield return new WaitForSeconds(5f);

        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(false);
        }
    }
}
