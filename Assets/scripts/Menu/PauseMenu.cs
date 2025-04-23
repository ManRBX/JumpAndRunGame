using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public PlayerShooting playerShooting;

    private bool isPaused = false;
    private float inputCooldown = 0.2f; // Schutz gegen Spam
    private float lastInputTime;

    void Update()
    {
        if (Time.unscaledTime - lastInputTime >= inputCooldown && Input.GetKeyDown(KeyCode.Escape))
        {
            lastInputTime = Time.unscaledTime;

            if (isPaused)
            {
                if (optionsPanel.activeSelf)
                {
                    CloseOptions();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        SetPauseState(true);
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        SetPauseState(false);
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
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

    private void SetPauseState(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;

        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;

        if (playerShooting != null)
        {
            playerShooting.enabled = !pause;
        }
    }
}
