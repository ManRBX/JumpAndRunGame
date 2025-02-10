using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;  // Haupt-Pausemenü
    public GameObject optionsPanel;  // Optionen-Panel
    public PlayerShooting playerShooting;  // Verweis auf PlayerShooting-Skript

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
        Time.timeScale = 0f;  // Spiel pausieren
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerShooting != null)
        {
            playerShooting.enabled = false;  // Schießen deaktivieren
        }
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;  // Spiel fortsetzen
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerShooting != null)
        {
            playerShooting.enabled = true;  // Schießen wieder aktivieren
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
