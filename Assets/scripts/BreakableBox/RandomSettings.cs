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
}
