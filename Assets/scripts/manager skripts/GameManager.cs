using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int playerLives = 3;

    private void Awake()
    {
        // Singleton pattern, but without DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // Destroys duplicate instances when the scene restarts
        }
    }

    public void PlayerDied()
    {
        playerLives--;

        if (playerLives <= 0)
        {
            // If all lives are lost, load the Game Over scene
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // Respawn the player (e.g., at a checkpoint)
            RespawnPlayer();
        }
    }

    // Player respawn logic
    private void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // CheckpointManager is used to determine the respawn position
            Vector3 respawnPosition = CheckpointManager.instance != null
                ? CheckpointManager.instance.GetCheckpointPosition()
                : Vector3.zero;  // Fallback to origin

            respawnPosition.y += 1f;  // Spawn the player slightly above the ground
            player.transform.position = respawnPosition;

            Debug.Log("Player respawned at: " + respawnPosition);
        }
    }
}
