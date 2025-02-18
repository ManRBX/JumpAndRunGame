using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;  // Main pause menu panel
    public GameObject optionsPanel;  // Options panel
    public PlayerShooting playerShooting;  // Reference to PlayerShooting script

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;  // Pause the game
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerShooting != null)
        {
            playerShooting.enabled = false;  // Disable shooting while paused
        }
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerShooting != null)
        {
            playerShooting.enabled = true;  // Re-enable shooting
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload the current level
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");  // Load the main menu scene
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);  // Open the options menu
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);  // Close options and return to pause menu
    }
}
