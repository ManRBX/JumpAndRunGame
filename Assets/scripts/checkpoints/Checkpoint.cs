using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Get the reference to the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing from the checkpoint object!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CheckpointManager.instance != null)
            {
                // Set the current position as the new checkpoint
                CheckpointManager.instance.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint reached!");

                // Change the color to Blue
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
            }
            else
            {
                Debug.LogError("CheckpointManager is missing!");
            }
        }
    }
}
