using UnityEngine;

[System.Serializable]
public class PowerUpDrop
{
    public GameObject powerUpPrefab;      // z. B. SpeedBoost, Health, Ammo
    [Range(0f, 1f)]
    public float individualChance = 0.5f;  // Chance für dieses PowerUp
}
