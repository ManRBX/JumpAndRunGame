using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Referenz auf den SpriteRenderer holen
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer fehlt am Checkpoint-Objekt!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CheckpointManager.instance != null)
            {
                // Setze die aktuelle Position als neuen Checkpoint
                CheckpointManager.instance.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint erreicht!");

                // Farbe auf Blau ändern
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
            }
            else
            {
                Debug.LogError("CheckpointManager ist nicht vorhanden!");
            }
        }
    }
}
