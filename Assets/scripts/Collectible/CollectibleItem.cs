using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool collected = false;

    // Achte darauf, dass dieses GameObject einen 2D-Collider (z. B. BoxCollider2D oder CircleCollider2D)
    // besitzt und die Option "Is Trigger" aktiviert ist.

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
        Debug.Log("Ein Collectible wurde eingesammelt!");

        // Informiere den Manager, dass dieses Item eingesammelt wurde, sofern das MiniGame aktiv ist.
        if (MiniGameManager.Instance != null && MiniGameManager.Instance.miniGameActive)
        {
            MiniGameManager.Instance.CollectiblePicked();
        }

        // Deaktiviere das Item, damit es nicht erneut eingesammelt wird.
        gameObject.SetActive(false);
    }
}
