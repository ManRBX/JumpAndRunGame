// Beispiel Pickup
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public InventoryItem item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}
