using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel;

    [Header("Lives & Ammo Settings")]
    public int restartLives = 5;
    public int restartAmmo = 30;

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Munition direkt setzen, sobald Panel aufgeht
        PlayerPrefs.SetInt("GlobalAmmo", restartAmmo);
        PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");
        PlayerPrefs.Save();

        var shooter = FindFirstObjectByType<PlayerShooting>();
        if (shooter != null)
        {
            shooter.AddAmmo(restartAmmo);    // Munition setzen
            shooter.SetAmmoFromPrefs();      // UI updaten
        }

        Time.timeScale = 0f;
        Debug.Log("💀 Game Over Panel geöffnet.");
        Debug.Log($"🧨 Munition gesetzt auf {restartAmmo}");
    }

    public void ContinueWithFiveLives()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Leben zurücksetzen
        PlayerPrefs.SetInt("GlobalLives", restartLives);
        PlayerPrefsKeyTracker.TrackKey("GlobalLives");

        // Munition zurücksetzen
        PlayerPrefs.SetInt("GlobalAmmo", restartAmmo);
        PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");

        PlayerPrefs.Save();
        Debug.Log($"❤️ Leben auf {restartLives} & Munition auf {restartAmmo} zurückgesetzt!");

        // Spieler suchen & resetten
        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.currentHealth = player.maxHealth;
            player.StartCoroutine(player.ApplyInvincibility(1f));
            player.RespawnExtern(); // Muss public sein!
        }

        // Leben-UI aktualisieren
        var ui = FindFirstObjectByType<PlayerHealthUI>();
        if (ui != null)
            ui.UpdateLivesUI();

        // Ammo-UI aktualisieren
        var shooter = FindFirstObjectByType<PlayerShooting>();
        if (shooter != null)
            shooter.SetAmmoFromPrefs();

        // Panel schließen & Spiel fortsetzen
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        Debug.Log("🟢 Spiel wurde fortgesetzt mit vollem Leben & Munition.");
    }

    public void ReturnToMainMenu()
    {
        // Leben & Ammo zurücksetzen
        PlayerPrefs.SetInt("GlobalLives", restartLives);
        PlayerPrefs.SetInt("GlobalAmmo", restartAmmo);
        PlayerPrefsKeyTracker.TrackKey("GlobalLives");
        PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        Debug.Log($"🧨 Munition gesetzt auf {restartAmmo}");
    }
}
