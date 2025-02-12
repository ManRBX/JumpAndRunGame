using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool collected = false;

    // Make sure this GameObject has a 2D collider (e.g., BoxCollider2D or CircleCollider2D)
    // and that the "Is Trigger" option is enabled.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;
        Debug.Log("A collectible has been collected!");

        // Inform the manager that this item has been collected, if the MiniGame is active.
        if (MiniGameManager.Instance != null && MiniGameManager.Instance.miniGameActive)
        {
            MiniGameManager.Instance.CollectiblePicked();
        }

        // Disable the item so it cannot be collected again.
        gameObject.SetActive(false);
    }
}
