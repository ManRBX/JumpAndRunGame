using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Effects/Ammo")]
public class AmmoEffect : PowerUpEffect
{
    public int ammoAmount = 10;
    public int maxAmmo = 999;

    public override void ApplyEffect()
    {
        PlayerShooting shooter = GameObject.FindObjectOfType<PlayerShooting>();
        if (shooter != null)
        {
            int currentAmmo = shooter.GetCurrentAmmo();

            if (currentAmmo < maxAmmo)
            {
                int ammoToAdd = Mathf.Min(ammoAmount, maxAmmo - currentAmmo);
                shooter.AddAmmo(ammoToAdd);
                Debug.Log($"🔫 Munition hinzugefügt: +{ammoToAdd} (neu: {currentAmmo + ammoToAdd})");
            }
            else
            {
                Debug.Log("⚠️ Munition ist bereits voll (999), PowerUp wird nicht verwendet.");
            }
        }
        else
        {
            Debug.LogWarning("❌ Kein PlayerShooting gefunden zum Munition hinzufügen!");
        }
    }
}
