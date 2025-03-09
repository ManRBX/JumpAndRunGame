using UnityEngine;

[CreateAssetMenu(fileName = "RandomSettings", menuName = "Settings/RandomSettings", order = 1)]
public class RandomSettings : ScriptableObject
{
    [Header("Coin Settings")]
    [Range(0f, 1f)]
    public float coinAwardChance = 0.5f;
    public int awardedCoins = 1;

    [Header("Fragment Settings")]
    [Range(0f, 1f)]
    public float fragmentSpawnChance = 1f;

    [Header("Fragment Cooldown Settings")]
    // The time (in seconds) that must pass before the same fragment can spawn again.
    public float fragmentCooldown = 20f;

    [Header("Fragment Visual Settings")]
    // If you want fragments to spawn with a random rotation:
    public bool randomizeFragmentRotation = true;
    public float minFragmentRotation = -15f;
    public float maxFragmentRotation = 15f;
}
