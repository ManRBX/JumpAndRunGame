using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    private Vector3 checkpointPosition;

    private void Awake()
    {
        // Singleton: Only one instance per scene
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // Remove duplicate instances
        }

        // Set a default position for the checkpoint
        checkpointPosition = transform.position;
    }

    // Sets the checkpoint to a new position
    public void SetCheckpoint(Vector3 newPosition)
    {
        checkpointPosition = newPosition;
        Debug.Log("Checkpoint set: " + checkpointPosition);
    }

    // Returns the current checkpoint position
    public Vector3 GetCheckpointPosition()
    {
        return checkpointPosition;
    }
}
