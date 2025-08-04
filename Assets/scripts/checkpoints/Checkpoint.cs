using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    [Header("🔖 Checkpoint Nachricht")]
    public string checkpointMessage = "Du hast einen Checkpoint erreicht!";
    public TMP_Text checkpointMessageText;
    public float messageDuration = 3f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer fehlt am Checkpoint-Objekt!");
        }

        if (checkpointMessageText != null)
            checkpointMessageText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CheckpointManager.instance != null)
            {
                CheckpointManager.instance.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint erreicht!");

                if (checkpointMessageText != null)
                    StartCoroutine(DisplayCheckpointMessage());
            }
            else
            {
                Debug.LogError("CheckpointManager fehlt!");
            }
        }
    }

    private System.Collections.IEnumerator DisplayCheckpointMessage()
    {
        checkpointMessageText.text = checkpointMessage;
        checkpointMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        checkpointMessageText.gameObject.SetActive(false);
    }
}
