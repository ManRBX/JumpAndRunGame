using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;  // Import TMP namespace

public class LevelEnd : MonoBehaviour
{
    public string nextLevelName;  // Name of the next level (e.g., "Level02")
    public string currentLevelName;  // Current level (e.g., "Level01")
    public string returnScene = "Menu";  // Scene to return to
    public int requiredSpecialCoins = 5; // Number of special coins required to unlock the next level

    public TMP_Text coinProgressText; // Reference to the TMP_Text element for showing coin progress

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
        // Get the number of special coins collected in this level
        int collectedSpecialCoins = PlayerPrefs.GetInt(currentLevelName + "-special-coins", 0);

        // Display the progress in the format "Collected/Required" (e.g., "1/5")
        if (coinProgressText != null)
        {
            coinProgressText.text = $"{collectedSpecialCoins}/{requiredSpecialCoins}";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the number of special coins collected in this level
            int collectedSpecialCoins = PlayerPrefs.GetInt(currentLevelName + "-special-coins", 0);

            // Check if the player has collected enough special coins
            if (collectedSpecialCoins >= requiredSpecialCoins)
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
            else
            {
                Debug.Log("Not enough special coins. You need " + requiredSpecialCoins + " special coins to proceed.");
                // Optionally show a message to the player, telling them they need more coins
            }

            // Show the coin progress UI and start a coroutine to hide it after 10 seconds
            ShowCoinProgress();
        }
    }

    void ShowCoinProgress()
    {
        // Update the coin progress UI
        UpdateCoinProgressUI();

        // Show the coin progress text
        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(true);
        }

        // Start a coroutine to hide the UI after 10 seconds
        StartCoroutine(HideCoinProgress());
    }

    IEnumerator HideCoinProgress()
    {
        // Wait for 10 seconds
        yield return new WaitForSeconds(5f);

        // Hide the coin progress UI after 10 seconds
        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(false);
        }
    }
}
