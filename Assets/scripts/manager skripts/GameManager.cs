using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int playerLives = 3;

    private void Awake()
    {
        // Singleton-Pattern, aber ohne DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // Zerstört doppelte Instanzen beim Szenen-Neustart
        }
    }

    public void PlayerDied()
    {
        playerLives--;

        if (playerLives <= 0)
        {
            // Wenn alle Leben verloren sind, lade Game Over Szene
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // Spieler respawnt (z.B. an Checkpoint)
            RespawnPlayer();
        }
    }

    // Spieler-Respawn-Logik
    private void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // CheckpointManager wird verwendet, um die Respawn-Position zu bestimmen
            Vector3 respawnPosition = CheckpointManager.instance != null
                ? CheckpointManager.instance.GetCheckpointPosition()
                : Vector3.zero;  // Fallback zu Ursprung

            respawnPosition.y += 1f;  // Spieler leicht über dem Boden spawnen
            player.transform.position = respawnPosition;

            Debug.Log("Spieler respawnt bei: " + respawnPosition);
        }
    }
}
