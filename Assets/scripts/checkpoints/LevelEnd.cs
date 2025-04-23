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
        if (currentLevelName == "Level01")
        {
            PlayerPrefs.SetInt(currentLevelName + "_Unlocked", 1);
            PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Unlocked");
        }

        PlayerPrefs.SetInt(currentLevelName + "_Visited", 1);
        PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Visited");

        PlayerPrefs.Save();
        Debug.Log(currentLevelName + " entered.");

        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(false);
        }
    }

    private void UpdateCoinProgressUI()
    {
        int collectedGlobalSpecialCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);

        if (coinProgressText != null)
        {
            coinProgressText.text = $"{collectedGlobalSpecialCoins}/{requiredSpecialCoins}";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int collectedGlobalSpecialCoins = PlayerPrefs.GetInt("GlobalSpecialCoins", 0);
            PlayerPrefsKeyTracker.TrackKey("GlobalSpecialCoins");

            if (collectedGlobalSpecialCoins >= requiredSpecialCoins)
            {
                PlayerPrefs.SetInt(currentLevelName + "_Completed", 1);
                PlayerPrefsKeyTracker.TrackKey(currentLevelName + "_Completed");
                Debug.Log(currentLevelName + " completed!");

                if (!string.IsNullOrEmpty(nextLevelName))
                {
                    PlayerPrefs.SetInt(nextLevelName + "_Unlocked", 1);
                    PlayerPrefsKeyTracker.TrackKey(nextLevelName + "_Unlocked");
                    Debug.Log(nextLevelName + " unlocked!");
                }

                PlayerPrefs.Save();
                SceneManager.LoadScene(returnScene);
            }
            else
            {
                Debug.Log($"❌ Not enough special coins! You need {requiredSpecialCoins}, but only have {collectedGlobalSpecialCoins}.");
            }

            ShowCoinProgress();
        }
    }

    void ShowCoinProgress()
    {
        UpdateCoinProgressUI();

        if (coinProgressText != null)
        {
            coinProgressText.gameObject.SetActive(true);
        }

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
