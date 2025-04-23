using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    private Vector3 checkpointPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        checkpointPosition = transform.position; // Default-Startpunkt
    }

    /// <summary>
    /// Speichert den Checkpoint nur im RAM – NICHT persistent!
    /// </summary>
    public void SetCheckpoint(Vector3 newPosition)
    {
        checkpointPosition = newPosition;
        Debug.Log("Checkpoint gesetzt (nicht gespeichert): " + checkpointPosition);
    }

    public Vector3 GetCheckpointPosition()
    {
        return checkpointPosition;
    }
}
