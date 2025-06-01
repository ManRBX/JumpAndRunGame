using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Effects/Health")]
public class HealthEffect : PowerUpEffect
{
    [Header("❤️ Heilungseffekt")]
    public int healAmount = 1;  // Anzahl Leben die hinzugefügt werden sollen

    public override void ApplyEffect()
    {
        PlayerHealth health = GameObject.FindObjectOfType<PlayerHealth>();
        if (health != null)
        {
            health.AddLife(healAmount);
        }
        else
        {
            Debug.LogWarning("❌ Kein PlayerHealth gefunden!");
        }
    }
}
