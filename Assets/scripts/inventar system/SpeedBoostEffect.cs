using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Effects/SpeedBoost")]
public class SpeedBoostEffect : PowerUpEffect
{
    public float boostAmount = 2f;
    public float duration = 5f;

    public override void ApplyEffect()
    {
        PlayerMovement player = GameObject.FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.StartCoroutine(player.ApplySpeedBoost(boostAmount, duration));
        }
    }
}
