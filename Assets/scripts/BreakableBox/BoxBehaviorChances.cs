using UnityEngine;

[System.Serializable]
public class BoxBehaviorChances
{
    [Range(0f, 1f)] public float shouldBreakChance = 1.0f;
    [Range(0f, 1f)] public float showDestructionEffectChance = 1.0f;
    [Range(0f, 1f)] public float startInvisibleChance = 0.0f;
}
