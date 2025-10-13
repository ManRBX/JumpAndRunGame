using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;
        Collect();
    }

    private void Collect()
    {
        Debug.Log("✅ Collectible eingesammelt!");
        gameObject.SetActive(false); // Nur deaktivieren, nicht löschen!

        if (MiniGameController.Instance != null && MiniGameController.Instance.miniGameActive)
            MiniGameController.Instance.CollectiblePicked();

        // Wenn es das letzte war -> Erfolgsmeldung anzeigen
        if (MiniGameController.Instance != null &&
            MiniGameController.Instance.CollectedCount >= MiniGameController.Instance.requiredCollectibles)
        {
            var starter = FindFirstObjectByType<Starter>();
            if (starter != null)
                starter.ShowMiniGameSuccessMessage();
        }
    }
}
