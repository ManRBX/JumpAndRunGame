using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public InventoryItem item;

    [Header("🔊 Pickup-Sound (AudioSource am Prefab)")]
    public AudioSource equipmentSound;

    private bool pickedUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        pickedUp = true;

        // Item ins Inventar
        InventoryManager.Instance.AddItem(item);

        // Collider deaktivieren, damit es nicht mehrfach triggert
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // Optional: Visuals ausblenden
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;

        // Sound korrekt spielen
        float delay = 0f;
        if (equipmentSound != null && equipmentSound.clip != null)
        {
            // AudioSource von diesem Objekt loslösen, damit Destroy es nicht killt
            equipmentSound.transform.SetParent(null, true);
            equipmentSound.Play();

            // Clip-Länge berücksichtigen (Pitch beachten)
            delay = equipmentSound.clip.length / Mathf.Max(0.0001f, equipmentSound.pitch);
        }
        else
        {
            Debug.LogWarning("⚠️ Equipment-Sound fehlt oder Clip nicht zugewiesen.");
        }

        // Ursprüngliches Pickup zerstören (nachdem Sound fertig ist)
        Destroy(gameObject, 0.05f);

        // Die gelöste AudioSource danach ebenfalls wegräumen
        if (equipmentSound != null)
        {
            Destroy(equipmentSound.gameObject, delay + 0.1f);
        }
    }
}
